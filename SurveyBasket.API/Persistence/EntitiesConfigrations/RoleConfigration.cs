using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.API.Abstractions.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class RoleConfigration : IEntityTypeConfiguration<ApplicationRole>
	{
		public void Configure(EntityTypeBuilder<ApplicationRole> builder)
		{
			//Default Data
			builder.HasData([
				new ApplicationRole
				{
					Id = DefaultRoles.AdminRoleId,
					Name = DefaultRoles.Admin,
					NormalizedName = DefaultRoles.Admin.ToUpper(),
					ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp
				},
				new ApplicationRole
				{
					Id = DefaultRoles.MemberRoleId,
					Name = DefaultRoles.Member,
					NormalizedName = DefaultRoles.Member.ToUpper(),
					ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
					IsDefault = true
				}
			]);

		}
	}
}
