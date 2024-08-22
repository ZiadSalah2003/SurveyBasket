using FluentValidation;

namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public class ResendConfirmationEmailRequestValidatior :  AbstractValidator<ResendConfirmationEmailRequest>
	{
        public ResendConfirmationEmailRequestValidatior()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
