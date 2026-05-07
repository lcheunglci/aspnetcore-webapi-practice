using System.Security.Claims;
using DishesAPI.DbContexts;
using DishesAPI.EndpointHandlers;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddValidation();

// Add services to the container.

// connection string from appSettings
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
	builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.MapGet("/testerror", () =>
{
	throw new NotImplementedException();
});

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

// recreate & migrate the database on each run, for demo purposes
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
	var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
	context.Database.EnsureDeleted();
	context.Database.Migrate();
}

app.Run();
