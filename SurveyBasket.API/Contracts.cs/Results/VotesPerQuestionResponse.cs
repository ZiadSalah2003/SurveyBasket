namespace SurveyBasket.API.Contracts.cs.Results
{
	public record VotesPerQuestionResponse(
		string Question,
		IEnumerable<VotesPerAnswerResponse> SelectedAnswers
	);
}
