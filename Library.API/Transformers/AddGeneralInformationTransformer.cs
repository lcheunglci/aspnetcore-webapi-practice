using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Library.API.Transformers
{
	public class AddGeneralInformationTransformer : IOpenApiDocumentTransformer
	{
		public Task TransformAsync(
			OpenApiDocument document, 
			OpenApiDocumentTransformerContext context, 
			CancellationToken cancellationToken)
		{
			document.Info = new()
			{
				Title = "Library API",
				Version = context.DocumentName,
				Description = "Through this API you can access authors and their books.",
				Contact = new()
				{
					Name = "Test Author",
					Email = "test@test.com"
				},
				License = new()
				{
					Name = "MIT License",
					Url = new Uri("https://opensource.org/licenses/MIT")
				}
			};
			return Task.CompletedTask;
		}
	}
}
