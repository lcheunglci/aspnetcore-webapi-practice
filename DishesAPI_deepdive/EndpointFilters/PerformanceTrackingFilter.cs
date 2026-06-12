using System.Diagnostics;

namespace DishesAPI.EndpointFilters
{
	public class PerformanceTrackingFilter : IEndpointFilter
	{
		private readonly ILogger<PerformanceTrackingFilter> _logger;

		public PerformanceTrackingFilter(ILogger<PerformanceTrackingFilter> logger)
		{
			_logger = logger;
		}

		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var stopwatch = Stopwatch.StartNew();
			var result = await next(context);
			stopwatch.Stop();
			var elapsedMs = ;
			_logger.LogInformation($"Endpoint {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} completed in {elapsedMs} ms.");
			return result;
		}
	}
}
