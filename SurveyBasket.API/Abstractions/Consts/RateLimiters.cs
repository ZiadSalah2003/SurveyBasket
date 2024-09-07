namespace SurveyBasket.API.Abstractions.Consts
{
	public static class RateLimiters
	{
		public const string IpLimiter = "ipLimit";
		public const string UserLimiter = "userLimit";
		public const string Concurrency = "concurrency";
		public const string Token = "token";
		public const string Fixed = "fixed";
		public const string Sliding = "sliding";
	}
}
