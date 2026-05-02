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
	var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
	return dishEntity?.ToDishDto();
});

app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, string dishName) =>
{
	var dishEntity = (await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName));
	return dishEntity?.ToDishDto();
});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext dishesDbContext, Guid dishId) =>
{
	var dishEntity = (await dishesDbContext.Dishes
		.Include(d => d.Ingredients)
		.FirstOrDefaultAsync(d => d.Id == dishId));

	return dishEntity?.Ingredients.ToIngredientDtoList(dishId);
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
