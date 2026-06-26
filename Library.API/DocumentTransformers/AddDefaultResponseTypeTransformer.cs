using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Library.API.DocumentTransformers
{
	public class AddDefaultResponseTypeTransformer : IOpenApiDocumentTransformer
	{
		public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
