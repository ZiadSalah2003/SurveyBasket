

namespace SurveyBasket.API.Errors
{
	public static class UserErrors
	{
		public static readonly Error InvalidCredentails = new("User.InvalidCredentials", "Invalid Email Or Password");
		public static readonly Error InvalidJwtToken = new("User.InvalidJwtToken", "Invalid Jwt token");

		public static readonly Error InvalidRefreshToken = new("User.InvalidRefreshToken", "Invalid refresh token");
	}
}
