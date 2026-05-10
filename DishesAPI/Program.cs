using System.Security.Claims;
using DishesAPI.DbContexts;
using DishesAPI.EndpointHandlers;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddValidation();

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddAuthorization();

// use dotnet user-jwts create --audience menu-api --role admin --claim country=Belgium
builder.Services.AddAuthorizationBuilder()
	.AddPolicy("RequireAdminFromBelgium", policy => policy.RequireAuthenticatedUser()
		.RequireClaim("country", "Belgium")
		.RequireRole("admin")
	);

// Add services to the container.

// connection string from appSettings
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
	builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

// openai/v1 endpoints for OpenAPI spec
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
	app.MapScalarApiReference();
}

// app.UseAuthentication();
// app.UseAuthorization();

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
