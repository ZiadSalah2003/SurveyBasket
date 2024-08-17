using FluentValidation;
using SurveyBasket.API.Contracts.cs.Polls;

namespace SurveyBasket.API.Contracts.cs.Questions
{
	public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
	{
		public QuestionRequestValidator()
		{
			RuleFor(x => x.Content)
				.NotEmpty()
				.Length(3, 1000);

			RuleFor(x => x.Answers)
				.NotNull();

			RuleFor(x => x.Answers)
				.Must(x => x.Count > 1)
				.WithMessage("Question should has at least 2 answers")
				.When(x => x.Answers != null);

			RuleFor(x => x.Answers)
				.Must(x => x.Distinct().Count() == x.Count)
				.WithMessage("You can not dublicated asnwer for the same question")
				.When(x => x.Answers != null);
		}
	}
}
