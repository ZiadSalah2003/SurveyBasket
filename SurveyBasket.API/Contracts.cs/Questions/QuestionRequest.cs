namespace SurveyBasket.API.Contracts.cs.Questions
{
	public record QuestionRequest(
		string Content,
		List<string> Answers
		);
}
