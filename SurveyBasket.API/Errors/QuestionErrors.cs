namespace SurveyBasket.API.Errors
{
	public class QuestionErrors
	{
		public static readonly Error QuestionNotFound = new("Question.NotFound", "No Question was found with the given id", StatusCodes.Status404NotFound);
		public static readonly Error DuplicatedQuestionContent = new("Question.DuplicatedContent", "Another Question with the same Content is already exists", StatusCodes.Status409Conflict);
	}
}
