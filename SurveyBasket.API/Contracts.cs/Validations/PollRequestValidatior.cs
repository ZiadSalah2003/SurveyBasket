using FluentValidation;
using SurveyBasket.API.Contracts.cs.Requests;

namespace SurveyBasket.API.Contracts.cs.Validations
{
	public class PollRequestValidatior : AbstractValidator<PollRequest>
	{
        public PollRequestValidatior()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(3, 100);

			RuleFor(x => x.Summary)
			   .NotEmpty()
			   .Length(3, 1000);

			RuleFor(x => x.StartsAt)
			   .NotEmpty()
			   .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

			RuleFor(x => x.EndsAt)
			   .NotEmpty();
			RuleFor(x => x)
				.Must(HasValidDate)
				.WithName(nameof(PollRequest.EndsAt))
				.WithMessage("{PropertyName} should Greater Than or Equal Starts Date");

		}
		private bool HasValidDate(PollRequest pollRequest)
		{
			return pollRequest.EndsAt >= pollRequest.StartsAt;
		}
    }
}
