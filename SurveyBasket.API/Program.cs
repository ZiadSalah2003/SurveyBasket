using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.API;
using SurveyBasket.API.Persistence;
using SurveyBasket.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddIdentityApiEndpoints<ApplicationUser>();

// Add services to the container.


builder.Services.AddDependencies(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.UseExceptionHandler();
app.Run();
