using Mapster;
using SurveyBasket.API.Contracts.cs.Questions;
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
		}
	}
}
