namespace SurveyBasket.API.Contracts.cs.Users
{
	public record UpdateUserRequest(
		string FirstName,
		string LastName,
		string Email,
		IList<string> Roles
	);
}
