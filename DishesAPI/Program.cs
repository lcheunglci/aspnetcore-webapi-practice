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

app.MapGet("/testerror", () =>
{
	throw new NotImplementedException();
});


var dishesEndpoints = app.MapGroup("dishes");
var dishesWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");
var ingredientsEndpoints = dishesWithGuidIdEndpoints.MapGroup("/ingredients");


dishesEndpoints.MapGet("", async Task<Ok<IEnumerable<DishDto>>> (DishesDbContext dishesDbContext,
	ClaimsPrincipal claimsPrincipal,
	[FromQuery] string? name) =>
{
	Console.WriteLine($"User authenticated: {claimsPrincipal.Identity?.IsAuthenticated}");

	return TypedResults.Ok((await dishesDbContext.Dishes
	.Where(d => name == null || d.Name.Contains(name))
	.ToListAsync()).ToDishDtoList());
});

dishesWithGuidIdEndpoints.MapGet("", async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext dishesDbContext, Guid dishId) =>
{
	var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}
	return TypedResults.Ok(dishEntity.ToDishDto());
}).WithName("GetDish");

dishesEndpoints.MapGet("/{dishName}", async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext dishesDbContext, string dishName) =>
{
	var dishEntity = (await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName));
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}
	return TypedResults.Ok(dishEntity.ToDishDto());
});

ingredientsEndpoints.MapGet("", 
	async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> (DishesDbContext dishesDbContext, Guid dishId) =>
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


dishesEndpoints.MapPost("", async Task<CreatedAtRoute<DishDto>> (DishesDbContext dishesDbContext, [FromBody] DishForCreationDto dishForCreationDto) =>
{
	var dishEntity = dishForCreationDto.ToDish();
	dishesDbContext.Add(dishEntity);
	await dishesDbContext.SaveChangesAsync();
	var dishToReturn = dishEntity.ToDishDto();

	return TypedResults.CreatedAtRoute(dishToReturn, "GetDish", new { dishId = dishToReturn.Id });
});

dishesWithGuidIdEndpoints.MapPut("", async Task<Results<NotFound, NoContent>> (DishesDbContext dishesDbContext,
	Guid dishId,
	DishForUpdateDto dishForUpdateDto) =>
{
	var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}
	dishEntity.UpdateFromDto(dishForUpdateDto);
	await dishesDbContext.SaveChangesAsync();

	return TypedResults.NoContent();
});

dishesWithGuidIdEndpoints.MapDelete("", async Task<Results<NotFound, NoContent>> (DishesDbContext dishesDbContext,
	Guid dishId) =>
{
	var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
	if (dishEntity == null)
	{
		return TypedResults.NotFound();
	}
	dishesDbContext.Dishes.Remove(dishEntity);
	await dishesDbContext.SaveChangesAsync();

	return TypedResults.NoContent();
});

// recreate & migrate the database on each run, for demo purposes
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
	var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
	context.Database.EnsureDeleted();
	context.Database.Migrate();
}

app.Run();
