using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.API.Authentication;
using SurveyBasket.API.Persistence;
using SurveyBasket.API.Services;
using System.Reflection;
using System.Text;

namespace SurveyBasket.API
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddControllers();
			services.AddAuthConfig();

			var connectionStrings = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String DefaultConnection not found.");
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionStrings));

			services
				.AddSwaggerServices()
				.AddFluentValidationConfig();


			services.AddScoped<IPollService, PollService>();
			services.AddScoped<IAuthService, AuthService>();

			return services;
		}
		private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
		{
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();

			return services;
		}
		private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
		{
			services
				.AddFluentValidationAutoValidation()
				.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			return services;
		}

		private static IServiceCollection AddAuthConfig(this IServiceCollection services)
		{
			services.AddSingleton<IJwtProvider, JwtProvider>();

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(o =>
				{
					o.SaveToken = true;
					o.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
						ValidIssuer = jwtSettings?.Issuer,
						ValidAudience = jwtSettings?.Audience
					};
				});

			return services;


		}

	}
}
