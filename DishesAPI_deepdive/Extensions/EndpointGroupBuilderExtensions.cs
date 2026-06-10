using DishesAPI.Endpoints;

namespace DishesAPI.Extensions
{
	public static class EndpointGroupBuilderExtensions
	{
		public static IEndpointRouteBuilder MapEndpoints(
			this IEndpointRouteBuilder endpointRouteBuilder)
		{
			foreach (IEndpointDefinition endpointDefinition in endpointRouteBuilder.ServiceProvider.GetServices<IEndpointDefinition>())
			{
				endpointDefinition.RegisterEndpoints(endpointRouteBuilder);
			}

			return endpointRouteBuilder;
		}
	}
}
