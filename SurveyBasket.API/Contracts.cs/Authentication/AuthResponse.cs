namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public class AuthResponse
	{
		public string Id { get; set; }
		public string? Email { get; init; }
		public string? FirstName { get; init; }
		public string? LastName { get; init; }
		public string Token { get; init; }
		public int ExpiresIn { get; init; }
		public AuthResponse(string id, string? email,string firstName,string lastName, string token, int expiresIn)
		{
			Id = id;
			Email = email;
			FirstName = firstName;
			LastName = lastName;
			Token = token;
			ExpiresIn = expiresIn;
		}
	}
}
