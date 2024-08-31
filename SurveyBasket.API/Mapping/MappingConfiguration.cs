using Mapster;
using SurveyBasket.API.Contracts.cs.Questions;
using SurveyBasket.API.Contracts.cs.Users;
using SurveyBasket.API.Entities;

namespace SurveyBasket.API.Mapping
{
	public class MappingConfiguration : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<QuestionRequest, Question>()
				.Map(dest => dest.Answers, src => src.Answers.Select(answers => new Answer { Content = answers }));

			config.NewConfig<RegisterRequest, ApplicationUser>()
			    .Map(dest => dest.UserName, src => src.Email);

			config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
				.Map(dest => dest, src => src.user)
				.Map(dest => dest.Roles, src => src.roles);
		}
	}
}
