using DishesAPI.EndpointHandlers;

namespace DishesAPI.Endpoints
{
	public class IngredientsEndpointDefinitions : IEndpointDefinition
	{
		public void RegisterEndpoints(IEndpointRouteBuilder builder)
		{
			var ingredientsEndpoints = builder
				.MapGroup("/dishes/{dishId:guid}/ingredients")
				.RequireAuthorization()
				.WithTags("Ingredients");

			ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
		}
	}
}
