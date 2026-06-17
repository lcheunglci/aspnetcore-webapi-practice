using DishesAPI.Attributes;
using DishesAPI.EndpointFilters;
using DishesAPI.EndpointHandlers;

namespace DishesAPI.Endpoints
{
	public class DishesEndpointDefinitions : IEndpointDefinition
	{
		public void RegisterEndpoints(IEndpointRouteBuilder builder)
		{
			var versionSet = builder.NewApiVersionSet()
				.HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
				.ReportApiVersions()
				.Build();

			//var apiGroup = endpointRouteBuilder.MapGroup("/api")
			//	.RequireAuthorization();

			var dishesEndpoints = builder.MapGroup("/v{version:apiVersion}/dishes")
				.RequireAuthorization()
				.WithTags("Dishes")
				.WithGroupName("v1")
				.WithApiVersionSet(versionSet)
				.MapToApiVersion(new Asp.Versioning.ApiVersion(1, 0))
				.AddEndpointFilter<PerformanceTrackingFilter>();

			var dishWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}")
				.AddEndpointFilter<LogNotFoundResponseFilter>();

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
				// .RequireAuthorization("RequireAdminFromBelgium")
				.WithSummary("Create a dish")
				.WithDescription("Creates a new dish.  Requires the admin role and country Belgium.")
				.ProducesValidationProblem(400)
				.AddEndpointFilter<ValidateAnnotationsFilter>();

			dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishAsync)
				// .RequireAuthorization("RequireAdminFromBelgium")
				.AddEndpointFilter<DishIsLockedFilter>();

			dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishAsync)
				// .RequireAuthorization("RequireAdminFromBelgium")

				.AddEndpointFilter<DishIsLockedFilter>();

			dishesEndpoints.MapGet("/experimental", () => { throw new NotImplementedException(); })
				.WithMetadata(new ExperimentalAttribute());

			dishesEndpoints.MapGet("/stream", DishesStreamingHandlers.GetDishesStreamAsync)
				.WithSummary("Stream all dishes")
				.WithDescription(
					"Returns all dishes as a streamed JSON array" +
					"using IAsyncEnumerable. " +
					"This response uses chunked transfer encoding.");

			dishesEndpoints.MapGet("/sse", DishesStreamingHandlers.GetDishesSseStream)
			.WithSummary("Get dishes as server-sent events")
			.WithDescription(
				"Returns all dishes as a server-sent event stream" +
				"(text/event-stream). Each dish is sent as a " +
				"'dish' event with JSON data. A final 'done' " +
				"event signals the end of the stream.");

		}


	}
}
