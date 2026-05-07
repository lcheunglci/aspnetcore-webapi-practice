using DishesAPI.EndpointHandlers;

namespace DishesAPI.Extensions
{
	public static class EndpointRouterBuilderExtensions
	{
		public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
		{
			var dishesEndpoints = endpointRouteBuilder.MapGroup("dishes");
			var dishesWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");


			dishesEndpoints.MapGet("", DishesHandlers.GetDishesAsync);
			dishesWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync).WithName("GetDish");
			dishesEndpoints.MapGet("/{dishName}", DishesHandlers.GetDishByNameAsync);

			dishesEndpoints.MapPost("", DishesHandlers.CreateDishAsync);
			dishesWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync);
			dishesWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync);
		}

		public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
		{
			var ingredientsEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients");
			ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
		}
	}
}
