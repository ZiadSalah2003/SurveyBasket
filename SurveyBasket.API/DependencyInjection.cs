using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.API.Persistence;
using SurveyBasket.API.Services;
using System.Reflection;

namespace SurveyBasket.API
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddControllers();

			var connectionStrings = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection String DefaultConnection not found.");
			services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionStrings));

			services
				.AddSwaggerServices()
				.AddFluentValidationConfig();


			services.AddScoped<IPollService, PollService>();

			return services;
		}
		public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
		{
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();

			return services;
		}
		public static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
		{
			services
				.AddFluentValidationAutoValidation()
				.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			return services;
		}

	}
}
