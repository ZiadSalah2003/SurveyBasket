﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class PollConfigration : IEntityTypeConfiguration<Poll>
	{
		public void Configure(EntityTypeBuilder<Poll> builder)
		{
			builder.HasIndex(x=>x.Title).IsUnique();
			builder.Property(x=>x.Title).HasMaxLength(100);
			builder.Property(x=>x.Summary).HasMaxLength(1500);
			 
		}
	}
}
