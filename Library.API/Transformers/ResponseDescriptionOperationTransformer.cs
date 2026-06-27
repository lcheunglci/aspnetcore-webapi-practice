using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi;

namespace Library.API.DocumentTransformers
{
	public class ResponseDescriptionOperationTransformer : IOpenApiOperationTransformer
	{
		public Task TransformAsync(
			OpenApiOperation operation,
			OpenApiOperationTransformerContext context,
			CancellationToken cancellationToken)
		{
			if (operation.Responses is null) return Task.CompletedTask;

			foreach (var (statusCode, response) in operation.Responses)
			{
				if (IsDefaultOrEmptyDescription(statusCode, response.Description))
				{
					response.Description = statusCode switch
					{
						"200" => "The request was successful.",
						"201" => "The resource was created. The Location of the header contains the URI of the new resource.",
						"204" => "No content - the operation completed successfully.",
						"400" => "Bad request - the input failed validation. The response body contains a Problem Details object.",
						"401" => "Unauthorized - a valid Bearer token is required.",
						"403" => "Forbidden - the token lacks the required role or claims.",
						"404" => "Not found - the requested resource does not exist. The response body contains a Problem Details object.",
						_ => response.Description
					};
				}
			}
			return Task.CompletedTask;
		}

		private bool IsDefaultOrEmptyDescription(string statusCode, string? description)
		{
			if (string.IsNullOrWhiteSpace(description))
			{
				return true;
			}

			if (!int.TryParse(statusCode, out var code))
			{
				return false;
			}

			var defaultDescription = ReasonPhrases.GetReasonPhrase(code);
			return string.Equals(description, defaultDescription, StringComparison.OrdinalIgnoreCase);
		}
	}
}
