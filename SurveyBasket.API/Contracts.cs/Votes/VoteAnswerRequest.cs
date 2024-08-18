namespace SurveyBasket.API.Contracts.cs.Votes
{
	public record VoteAnswerRequest(
		int QuestionId,
		int AnswerId
	);
}
