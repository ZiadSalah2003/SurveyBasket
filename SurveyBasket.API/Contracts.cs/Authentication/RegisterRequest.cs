namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public record RegisterRequest(
		string Email,
		string Password,
		string FirstName,
		string LastName
	);
}
