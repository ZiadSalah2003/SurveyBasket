namespace SurveyBasket.API.Contracts.cs.Results
{
	public record PollVotesResponse(
	string Title,
	IEnumerable<VoteResponse> Votes
	);
}
