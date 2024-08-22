using FluentValidation;

namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
	{
        public ConfirmEmailRequestValidator()
        {
             RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.Code)
				.NotEmpty();
        }
    }
}
