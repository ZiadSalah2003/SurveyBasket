using FluentValidation;
using SurveyBasket.API.Contracts.cs.Questions;

namespace SurveyBasket.API.Contracts.cs.Votes
{
	public class VoteAnswerRequestValidator : AbstractValidator<VoteAnswerRequest>
	{
        public VoteAnswerRequestValidator()
        {
            RuleFor(x => x.QuestionId)
				.GreaterThan(0);

			RuleFor(x => x.AnswerId)
				.GreaterThan(0);
		}
    }
}
