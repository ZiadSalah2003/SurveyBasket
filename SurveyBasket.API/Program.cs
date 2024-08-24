using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SurveyBasket.API;
using SurveyBasket.API.Persistence;
using SurveyBasket.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddIdentityApiEndpoints<ApplicationUser>();

// Add services to the container.

builder.Host.UseSerilog((context, configuration) =>
	configuration.ReadFrom.Configuration(context.Configuration)
);

builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
	Authorization =
	[
		new HangfireCustomBasicAuthenticationFilter{
			User=app.Configuration.GetValue<string>("HangfireSettings:Username"),
			Pass=app.Configuration.GetValue<string>("HangfireSettings:Password")
		}
	],
	DashboardTitle = "SurveyBasket Dashboard",
	//IsReadOnlyFunc = (DashboardContext context) => true
});
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollsNotification", () => notificationService.SendNewPollsNotification(null), Cron.Daily);

app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler();
app.Run();
