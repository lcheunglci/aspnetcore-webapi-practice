using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace DishesAPI.EndpointFilters
{
	public class ValidateAnnotationsFilter : IEndpointFilter
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			foreach (var argument in context.Arguments)
			{
				if (argument is null) continue;

				var validateionResults = new List<ValidationResult>();
				var validationContext = new ValidationContext(argument);

				if (!Validator.TryValidateObject(argument, validationContext, validateionResults, true))
				{
					return TypedResults.ValidationProblem(
						validateionResults.ToDictionary(
						vr => vr.MemberNames.FirstOrDefault() ?? string.Empty,
						vr => new[] { vr.ErrorMessage! }));
				}

				if (argument is IValidatableObject validatableObject)
				{
					var customResults = validatableObject.Validate(validationContext).ToList();

					if (customResults.Count > 0 && customResults.Any(r => r != ValidationResult.Success))
					{
						return TypedResults.ValidationProblem(
							customResults.Where(r => r != ValidationResult.Success && r.ErrorMessage is not null)
								.ToDictionary(
									r => r.MemberNames.FirstOrDefault() ?? string.Empty,
									r => new[] { r.ErrorMessage! }));
					}

				}
			}
			return await next(context);
		}
	}
}
