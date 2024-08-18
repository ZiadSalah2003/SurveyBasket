using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class VoteConfigration : IEntityTypeConfiguration<Vote>
	{
		public void Configure(EntityTypeBuilder<Vote> builder)
		{
			builder.HasIndex(x => new { x.PollId, x.UserId }).IsUnique();
		}
	}
}
