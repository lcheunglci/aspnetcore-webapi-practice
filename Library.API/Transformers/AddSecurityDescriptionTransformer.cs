using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Library.API.Transformers
{
	public class AddSecurityDescriptionTransformer : IOpenApiDocumentTransformer
	{
		public Task TransformAsync(
			OpenApiDocument document, 
			OpenApiDocumentTransformerContext context, 
			CancellationToken cancellationToken)
		{
			document.Components ??= new OpenApiComponents();
			document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

			// Add Bearer token security scheme
			document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Description = "Input your Bearer token to access this API"
			});

			// Apply Security requirement globally
			document.Security = [
				new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecuritySchemeReference("Bearer"),
						[]
					}
				}
			];

			return Task.CompletedTask;
		}
	}
}
