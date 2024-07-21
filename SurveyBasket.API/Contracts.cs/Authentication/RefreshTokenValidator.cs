using FluentValidation;

namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
	{
		public RefreshTokenValidator()
		{
			RuleFor(x => x.Token).NotEmpty();
			RuleFor(x => x.RefreshToken).NotEmpty();
		}
	}
}
