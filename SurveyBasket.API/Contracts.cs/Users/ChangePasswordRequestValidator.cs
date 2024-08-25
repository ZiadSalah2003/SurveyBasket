using FluentValidation;
using SurveyBasket.API.Abstractions.Consts;

namespace SurveyBasket.API.Contracts.cs.Users
{
	public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
	{
		public ChangePasswordRequestValidator()
		{
			RuleFor(x => x.CurrentPassword)
				.NotEmpty();

			RuleFor(x => x.NewPassword)
				.NotEmpty()
				.Matches(RegexPatterns.Password)
				.WithMessage("Passwrod should be at least 8 digits and should contains lower case, nonalphanumeric and uppercase")
				.NotEqual(x => x.CurrentPassword)
				.WithMessage("New Passwrod can not be the same as the current password");
		}
	}
}
