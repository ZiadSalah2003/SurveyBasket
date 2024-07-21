namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public record RefreshTokenRequest(
		string Token,
		string RefreshToken
		);
}
