using FluentValidation;
using FluentValidation.AspNetCore;
using SurveyBasket.API;
using SurveyBasket.API.Contracts.cs.Requests;
using SurveyBasket.API.Contracts.cs.Validations;
using SurveyBasket.API.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
