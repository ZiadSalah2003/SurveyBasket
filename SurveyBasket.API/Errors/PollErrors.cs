﻿namespace SurveyBasket.API.Errors
{
	public static class PollErrors
	{
		public static readonly Error PollNotFound = new("Poll.NotFound", "No poll was found with the given id", StatusCodes.Status404NotFound);
		public static readonly Error DuplicatedPollTitle = new("Poll.DuplicatedTitle", "Poll with the same title is already exists", StatusCodes.Status409Conflict);
	}
}
