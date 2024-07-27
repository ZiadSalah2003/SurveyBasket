namespace SurveyBasket.API.Errors
{
	public static class PollErrors
	{
		public static readonly Error PollNotFound = new("Poll.NotFound", "No poll was found with the given id");
	}
}
