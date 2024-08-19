namespace SurveyBasket.API.Contracts.cs.Results
{
	public record VotesPerDayResponse(
	DateOnly Date,
	int NumberOfVotes
	);
}
