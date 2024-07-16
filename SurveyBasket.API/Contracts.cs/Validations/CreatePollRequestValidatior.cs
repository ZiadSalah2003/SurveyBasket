using FluentValidation;
using SurveyBasket.API.Contracts.cs.Requests;

namespace SurveyBasket.API.Contracts.cs.Validations
{
	public class CreatePollRequestValidatior : AbstractValidator<CreatePollRequest>
	{
        public CreatePollRequestValidatior()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(3, 100);

			RuleFor(x => x.Description)
			   .NotEmpty()
			   .Length(3, 1000);

		}
    }
}
