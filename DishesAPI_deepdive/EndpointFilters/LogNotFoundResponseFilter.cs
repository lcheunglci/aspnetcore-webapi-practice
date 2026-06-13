namespace DishesAPI.EndpointFilters
{
	public class LogNotFoundResponseFilter : IEndpointFilter
	{
		private readonly ILogger<LogNotFoundResponseFilter> _logger;

		public LogNotFoundResponseFilter(ILogger<LogNotFoundResponseFilter> logger)
		{
			_logger = logger;
		}

		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var result = await next(context);
			var actualResult = result;
			while (actualResult is INestedHttpResult nestedResult)
			{
				actualResult = nestedResult.Result;
			}
			if (actualResult is IStatusCodeHttpResult {  StatusCode: StatusCodes.Status404NotFound})
			{
				_logger.LogWarning(
					"Resource not found: {Method} {Path}",
					context.HttpContext.Request.Method,
					context.HttpContext.Request.Path
				);
			}
			return result;
		}
	}
}
