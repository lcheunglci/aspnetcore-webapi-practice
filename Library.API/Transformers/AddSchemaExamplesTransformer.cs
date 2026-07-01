using System.Text.Json.Nodes;
using Library.API.Models;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Library.API.DocumentTransformers
{
	public class AddSchemaExamplesTransformer : IOpenApiSchemaTransformer
	{
		public Task TransformAsync(
			OpenApiSchema schema,
			OpenApiSchemaTransformerContext context,
			CancellationToken cancellationToken)
		{

			if (context.JsonTypeInfo.Type == typeof(Author))
			{
				schema.Example = JsonNode.Parse($$"""
				{
					"id": "{Guid.NewGuid()}",
					"firstName": "John",
					"lastName": "Doe"
				}
				""");
			}
			else if (context.JsonTypeInfo.Type == typeof(AuthorForUpdate))
			{
				schema.Example = JsonNode.Parse("""
				{
					"firstName": "John",
					"lastName": "Doe"
				}
				""");
			}
			else if (context.JsonTypeInfo.Type == typeof(Book))
			{
				schema.Example = JsonNode.Parse($$"""
				{
					"id": "{Guid.NewGuid()}",
					"title": "Sample Book Title",
					"description": "This is a sample book description."
				}
				""");
			}
			else if (context.JsonTypeInfo.Type == typeof(BookForCreation))
			{
				schema.Example = JsonNode.Parse("""
				{
					"title" : "Sample Book Title",
					"description": "This is a sample book description."
				}
				""");
			}

			return Task.CompletedTask;
		}
	}
}
