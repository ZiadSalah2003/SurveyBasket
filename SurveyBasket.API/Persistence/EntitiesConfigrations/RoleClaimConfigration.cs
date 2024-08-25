using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.API.Abstractions.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class RoleClaimConfigration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
		{
			//Default Data
			var permissions = Permissions.GetAllPermissions();
			var adminClaims = new List<IdentityRoleClaim<string>>();

			for (int i = 0; i < permissions.Count; i++)
			{
				adminClaims.Add(new IdentityRoleClaim<string>
				{
					Id = i + 1,
					ClaimType = Permissions.Type,
					ClaimValue = permissions[i],
					RoleId = DefaultRoles.AdminRoleId
				});
			}

			builder.HasData(adminClaims);
		}
	}
}
