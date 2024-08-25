namespace SurveyBasket.API.Contracts.cs.Users
{
	public record ChangePasswordRequest(
		string CurrentPassword,
		string NewPassword
	);
}
