namespace SurveyBasket.API.Contracts.cs.Votes
{
	public record VoteRequest(
		IEnumerable<VoteAnswerRequest> Answers
	);
}
