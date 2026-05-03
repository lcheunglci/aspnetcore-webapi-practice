using System.Security.Claims;
using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

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


app.MapGet("/dishes", async Task<Ok<IEnumerable<DishDto>>>(DishesDbContext dishesDbContext,
	ClaimsPrincipal claimsPrincipal,
	[FromQuery] string? name) =>
{
	Console.WriteLine($"User authenticated: {claimsPrincipal.Identity?.IsAuthenticated}");

	return TypedResults.Ok((await dishesDbContext.Dishes
	.Where(d => name == null || d.Name.Contains(name))
	.ToListAsync()).ToDishDtoList());
});

app.MapGet("/dishes/{dishId:guid}", async Task<Results<NotFound, Ok<DishDto>>>(DishesDbContext dishesDbContext, Guid dishId) =>
{
	var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}
	return TypedResults.Ok(dishEntity.ToDishDto());
});

app.MapGet("/dishes/{dishName}", async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext dishesDbContext, string dishName) =>
{
	var dishEntity = (await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName));
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}
	return TypedResults.Ok(dishEntity.ToDishDto());
});

app.MapGet("/dishes/{dishId}/ingredients", async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> (DishesDbContext dishesDbContext, Guid dishId) =>
{
	var dishEntity = (await dishesDbContext.Dishes
		.Include(d => d.Ingredients)
		.FirstOrDefaultAsync(d => d.Id == dishId));
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}

	return TypedResults.Ok(dishEntity?.Ingredients.ToIngredientDtoList(dishId));
});


using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
	var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
	context.Database.EnsureDeleted();
	context.Database.Migrate();
}

app.Run();
