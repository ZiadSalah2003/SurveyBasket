using FluentValidation;

namespace SurveyBasket.API.Contracts.cs.Users
{
	public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
	{
		public CreateUserRequestValidator()
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();

			RuleFor(x => x.Password)
				.NotEmpty()
				.Matches(RegexPatterns.Password)
				.WithMessage("Passwrod should be at least 8 digits and should contains lower case, nonalphanumeric and uppercase");

			RuleFor(x => x.FirstName)
				.NotEmpty()
				.Length(3, 100);

			RuleFor(x => x.LastName)
				.NotEmpty()
				.Length(3, 100);

			RuleFor(x => x.Roles)
			   .NotNull()
			   .NotEmpty();

			RuleFor(x => x.Roles)
				.Must(x => x.Distinct().Count() == x.Count)
				.WithMessage("You can not duplicate Role for the same user")
				.When(x => x.Roles != null);


		}
	}
}
