using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.API.Abstractions.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class UserRoleConfigration : IEntityTypeConfiguration<IdentityUserRole<string>>
	{
		public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
		{
			//Default Data
			builder.HasData(new IdentityUserRole<string>
			{
				UserId = DefaultUsers.AdminId,
				RoleId = DefaultRoles.AdminRoleId
			});
		}
	}
}
