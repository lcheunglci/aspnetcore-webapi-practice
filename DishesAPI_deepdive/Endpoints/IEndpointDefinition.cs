namespace DishesAPI.Endpoints
{
	public interface IEndpointDefinition
	{
		void RegisterEndpoints(IEndpointRouteBuilder builder);
	}
}
