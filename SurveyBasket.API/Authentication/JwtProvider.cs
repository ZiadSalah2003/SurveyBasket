﻿
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.API.Authentication
{
	public class JwtProvider : IJwtProvider
	{
		private readonly JwtOptions _jwtOptions;
		public JwtProvider(IOptions<JwtOptions> jwtOptions)
		{
			_jwtOptions = jwtOptions.Value;
		}
		public (string token, int expiresIn) GenerateToken(ApplicationUser user)
		{
			Claim[] claims = new Claim[]
			{
				new(JwtRegisteredClaimNames.Sub,user.Id),
				new(JwtRegisteredClaimNames.Email,user.Email!),
				new(JwtRegisteredClaimNames.GivenName,user.FirstName),
				new(JwtRegisteredClaimNames.FamilyName,user.LastName),
				new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
			};
			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
			var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: _jwtOptions.Issuer,
				audience: _jwtOptions.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
				signingCredentials: singingCredentials
				);
			return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: _jwtOptions.ExpiryMinutes * 60);
		}

		public string? ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
			try
			{
				tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					IssuerSigningKey = symmetricSecurityKey,
					ValidateIssuerSigningKey = true,
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);
				var jwtToken = (JwtSecurityToken)validatedToken;
				return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
			}
			catch
			{
				return null;
			}
		}
	}
}
