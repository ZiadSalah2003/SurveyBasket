﻿

namespace SurveyBasket.API.Errors
{
	public static class UserErrors
	{
		public static readonly Error InvalidCredentails = new("User.InvalidCredentials", "Invalid Email Or Password", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidJwtToken = new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidRefreshToken = new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

		public static readonly Error DuplicatedEmail = new("User.DuplicatedEmail", "Another user with the same email is already exists", StatusCodes.Status409Conflict);

		public static readonly Error EmailNotConfirmed =new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

		public static readonly Error InvalidCode =new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

		public static readonly Error DuplicatedConfirmation =new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);

		public static readonly Error DisabledUser = new("User.DisabledUser", "Disabled User please contact your adminstrator", StatusCodes.Status401Unauthorized);
		
		public static readonly Error UserNotFound = new("User.UserNotFound", "User is not found", StatusCodes.Status404NotFound);

		public static readonly Error InvalidRoles = new("User.InvalidRoles", "Invalid Roles", StatusCodes.Status400BadRequest);
	}
}
