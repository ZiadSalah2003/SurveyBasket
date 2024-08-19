namespace SurveyBasket.API.Contracts.cs.Results
{
	public record VoteResponse(
	string VoteName,
	DateTime VoteDate,
	IEnumerable<QuestionAnswerResponse> SelectedAnswers
	);
}
