using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SurveyBasket.API.Persistence.EntitiesConfigrations
{
	public class AnswerConfigration : IEntityTypeConfiguration<Answer>
	{
		public void Configure(EntityTypeBuilder<Answer> builder)
		{
			builder.HasIndex(x => new { x.QuestionId, x.Content }).IsUnique();
			builder.Property(x=>x.Content).HasMaxLength(1000);
		}
	}
}
