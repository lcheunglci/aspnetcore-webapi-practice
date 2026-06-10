using System.Reflection;
using DishesAPI.Endpoints;

namespace DishesAPI.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection RegisterAllEndpoints(this IServiceCollection services)
		{
			// get current assembly
			var currentAssembly = Assembly.GetExecutingAssembly();
			var endpointDefinitions = currentAssembly.GetTypes().Where(t =>
			typeof(IEndpointDefinition).IsAssignableFrom(t) &&
			t != typeof(IEndpointDefinition) &&
			t.IsPublic &&
			!t.IsAbstract);

			// register them
			foreach (var endpointDefinition in endpointDefinitions)
			{
				services.AddSingleton(typeof(IEndpointDefinition), endpointDefinition);
			}

			return services;
		}
	}
}
