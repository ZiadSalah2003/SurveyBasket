using SurveyBasket.API.Contracts.cs.Answers;

namespace SurveyBasket.API.Contracts.cs.Questions
{
	public record QuestionResponse(
		int Id,
		string Content,
		IEnumerable<AnswerResponse> Answers
		);
}
