using DishesAPI.Attributes;
using DishesAPI.EndpointHandlers;

namespace DishesAPI.Extensions;

public static class EndpointRouterBuilderExtensions
{
    public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
		//var apiGroup = endpointRouteBuilder.MapGroup("/api")
		//	.RequireAuthorization();

        var dishesEndpoints = endpointRouteBuilder.MapGroup("/dishes")
            .RequireAuthorization()
            .WithTags("Dishes");

        var dishWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");

        dishesEndpoints.MapGet("", DishesHandlers.GetDishesAsync)
            .WithSummary("Get all dishes")
            .WithDescription("Returns all dishes, optionally filtered by name.");
 
        dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync)
            .WithName("GetDish");
        dishesEndpoints.MapGet("/{dishName}", DishesHandlers.GetDishByNameAsync)
            .AllowAnonymous()
            .WithSummary("Get a dish by name")
            .WithDescription(
                "Returns a single dish identified by its name.  " +
                "This endpoint allows anonymous access.  ");
        ;
        dishesEndpoints.MapPost("", DishesHandlers.CreateDishAsync)
            .RequireAuthorization("RequireAdminFromBelgium")
            .WithSummary("Create a dish")
            .WithDescription("Creates a new dish.  Requires the admin role and country Belgium.")
            .ProducesValidationProblem(400);

        dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync)
            .RequireAuthorization("RequireAdminFromBelgium"); 
        dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync)
            .RequireAuthorization("RequireAdminFromBelgium");

		dishesEndpoints.MapGet("/experimental", () => { throw new NotImplementedException(); })
			.WithMetadata(new ExperimentalAttribute());

		//var isExperimental = context.EndpointMetadata
		//	OfType<ExperimentalAttribute>().Any();
    }

    public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientsEndpoints = endpointRouteBuilder
            .MapGroup("/dishes/{dishId:guid}/ingredients")
            .RequireAuthorization()
            .WithTags("Ingredients");

        ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
    }
}
