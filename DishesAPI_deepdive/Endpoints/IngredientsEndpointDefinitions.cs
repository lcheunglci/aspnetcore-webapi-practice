using DishesAPI.EndpointFilters;
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
				.WithTags("Ingredients")
				.AddEndpointFilter<PerformanceTrackingFilter>()
				.AddEndpointFilter<LogNotFoundResponseFilter>();

			ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
		}
	}
}
