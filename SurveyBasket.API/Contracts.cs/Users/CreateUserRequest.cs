namespace SurveyBasket.API.Contracts.cs.Users
{
	public record CreateUserRequest(
		string FirstName,
		string LastName,
		string Email,
		string Password,
		IList<string> Roles
	);
}
