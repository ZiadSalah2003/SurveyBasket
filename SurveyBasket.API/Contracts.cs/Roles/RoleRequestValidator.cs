using FluentValidation;

namespace SurveyBasket.API.Contracts.cs.Roles
{
	public class RoleRequestValidator : AbstractValidator<RoleRequest>
	{
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name)
				.NotEmpty()
				.Length(3,200);

            RuleFor(x => x.Permissions)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Permissions)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("You can not duplicate permission for the same role")
                .When(x => x.Permissions != null);

        }
    }
}
