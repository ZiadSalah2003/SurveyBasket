namespace SurveyBasket.API.Contracts.cs.Users
{
	public record UserResponse(
		string Id,	
		string FirstName,	
		string LastName,	
		string Email,	
		bool IsDisabled,
		IEnumerable<string> Roles
	);
}
