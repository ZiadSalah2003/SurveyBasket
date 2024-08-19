namespace SurveyBasket.API.Errors
{
	public static class AnswerErrors
	{
		public static readonly Error PollNotFound = new("Poll.NotFound", "No poll was found with the given id", StatusCodes.Status404NotFound);
	}
}
