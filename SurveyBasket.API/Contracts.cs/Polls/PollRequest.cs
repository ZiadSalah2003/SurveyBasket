namespace SurveyBasket.API.Contracts.cs.Polls
{
	public record PollRequest(
		 string Title,
		 string Summary,
		 DateOnly StartsAt,
		 DateOnly EndsAt
		);
}
