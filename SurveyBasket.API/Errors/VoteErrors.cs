namespace SurveyBasket.API.Errors
{
	public static class VoteErrors
	{
		public static readonly Error InvalidQuestion = new("Vote.InvalidQuestion", "Invalid Question", StatusCodes.Status400BadRequest);
		public static readonly Error DuplicatedVote = new("Vote.DuplicatedTitle", "this user already voted before in this Poll", StatusCodes.Status409Conflict);
	}
}
