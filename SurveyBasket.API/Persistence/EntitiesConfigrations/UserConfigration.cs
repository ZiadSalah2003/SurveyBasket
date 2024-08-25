﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SurveyBasket.API.Abstractions.Consts;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class UserConfigration : IEntityTypeConfiguration<ApplicationUser>
	{
		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			builder.OwnsMany(x => x.RefreshTokens).ToTable("RefreshTokens").WithOwner().HasForeignKey("UserId");

			builder.Property(x => x.FirstName).HasMaxLength(100);
			builder.Property(x => x.LastName).HasMaxLength(100);

			//Default Data

			var passwordHasher = new PasswordHasher<ApplicationUser>();

			builder.HasData(new ApplicationUser
			{
				Id = DefaultUsers.AdminId,
				FirstName = "Survay Basket",
				LastName = "Admin",
				UserName = DefaultUsers.AdminEmail,
				NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
				Email = DefaultUsers.AdminEmail,
				NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
				SecurityStamp = DefaultUsers.AdminSecurityStamp,
				ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
				EmailConfirmed = true,
				PasswordHash= passwordHasher.HashPassword(null!, DefaultUsers.AdminPassword)
			});
		}
	}
}
