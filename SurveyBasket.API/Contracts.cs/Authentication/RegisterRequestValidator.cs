﻿using FluentValidation;
using SurveyBasket.API.Abstractions.Consts;

namespace SurveyBasket.API.Contracts.cs.Authentication
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
        public RegisterRequestValidator()
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
		}
    }
}