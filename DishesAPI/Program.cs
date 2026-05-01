using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// connection string from appSettings
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
	builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();



app.MapGet("/dishes", async (DishesDbContext dishesDbContext) =>
{
	return (await dishesDbContext.Dishes.ToListAsync()).ToDishDtoList();
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext dishesDbContext, Guid dishId) =>
{
	return (await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId))?.ToDishDto();
});

app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, string dishName) =>
{
	return (await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName))?.ToDishDto();
});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext dishesDbContext, Guid dishId) =>
{
	var dishEntity = (await dishesDbContext.Dishes
		.Include(d => d.Ingredients)
		.FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients;

	return dishEntity?.ToIngredientDtoList(dishId);
});


using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
	var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
	context.Database.EnsureDeleted();
	context.Database.Migrate();
}

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
	public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
