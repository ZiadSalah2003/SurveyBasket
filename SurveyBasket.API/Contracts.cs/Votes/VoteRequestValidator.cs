using FluentValidation;
using SurveyBasket.API.Contracts.cs.Questions;

namespace SurveyBasket.API.Contracts.cs.Votes
{
	public class VoteRequestValidator : AbstractValidator<VoteRequest>
	{
        public VoteRequestValidator()
        {
            RuleFor(x => x.Answers)
				.NotNull();

            RuleForEach(x => x.Answers)
                .SetInheritanceValidator(v => v.Add(new VoteAnswerRequestValidator()));
        }
    }
}
