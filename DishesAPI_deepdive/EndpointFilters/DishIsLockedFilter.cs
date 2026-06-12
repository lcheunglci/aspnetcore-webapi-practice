namespace DishesAPI.EndpointFilters
{
	public class DishIsLockedFilter : IEndpointFilter
	{
		private readonly Guid _lockedDishId = Guid.Parse("fd630a57-2352-4731-b25c-db9cc7601b16");

		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var dishIdValue = context.HttpContext.Request.RouteValues["dishId"];
			if (dishIdValue is not null && Guid.TryParse(dishIdValue.ToString(), out var dishId))
			{
				if (dishId == _lockedDishId)
				{
					return TypedResults.Problem(
						statusCode: StatusCodes.Status403Forbidden,
						title: "Dish is locked",
						detail: $"This dish with id {dishId} is locked and cannot be modified.");

					//return new ValueTask<object?>(Results.Conflict(
					//	new { Message = "This dish is locked and cannot be modified." }));
				}
			}
			return await next(context);
		}
	}
}
