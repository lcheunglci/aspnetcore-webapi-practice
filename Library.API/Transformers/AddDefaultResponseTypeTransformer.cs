using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Library.API.DocumentTransformers
{
	public class AddDefaultResponseTypeTransformer : IOpenApiDocumentTransformer
	{
		public Task TransformAsync(
			OpenApiDocument document,
			OpenApiDocumentTransformerContext context,
			CancellationToken cancellationToken)
		{
			foreach (var pathItem in document.Paths.Values)
			{
				if (pathItem.Operations == null) continue;
				foreach (var operation in pathItem.Operations.Values)
				{
					if (operation.Responses == null) continue;
					if (!operation.Responses.ContainsKey("default"))
					{
						operation.Responses["default"] = new OpenApiResponse
						{
							Description = "An unexpected error occurred",
							Content = new Dictionary<string, OpenApiMediaType>
							{
								["application/json"] = new OpenApiMediaType
								{
									Schema = new OpenApiSchemaReference("ProblemDetails")
								}
							}
						};
					}

				}
			}
			return Task.CompletedTask;
		}
	}
}
