namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public record ConfirmEmailRequest(
		string UserId,
		string Code
	);
}
