namespace SurveyBasket.API.Contracts.cs.Results
{
	public record VotesPerAnswerResponse(
		string Answer,
		int count
	);
}
