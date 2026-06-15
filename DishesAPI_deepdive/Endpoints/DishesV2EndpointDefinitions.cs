using Asp.Versioning;
using DishesAPI.EndpointFilters;
using DishesAPI.EndpointHandlers;

namespace DishesAPI.Endpoints
{
	public class DishesV2EndpointDefinitions : IEndpointDefinition
	{
		public void RegisterEndpoints(IEndpointRouteBuilder builder)
		{
			var versionSet = builder.NewApiVersionSet()
				.HasApiVersion(new ApiVersion(1, 0))
				.ReportApiVersions()
				.Build();

			var dishesV2Endpoints = builder.MapGroup("/v{version:apiVersion}/dishes")
				.RequireAuthorization()
				.WithTags("Dishes")
				.WithGroupName("v2")
				.WithApiVersionSet(versionSet)
				.MapToApiVersion(new ApiVersion(2, 0))
				.AddEndpointFilter<PerformanceTrackingFilter>();


			var dishWithGuidIdEndpoints = dishesV2Endpoints.MapGroup("/{dishId:guid}")
				.AddEndpointFilter<LogNotFoundResponseFilter>();

			dishesV2Endpoints.MapGet("", DishesV2Handlers.GetDishesAsync)
				.WithSummary("Get all dishes")
				.WithDescription("Returns all dishes, optionally filtered by name.");

			dishWithGuidIdEndpoints.MapGet("", DishesV2Handlers.GetDishByIdAsync)
				.WithName("GetV2Dish");


		}
	}
}
