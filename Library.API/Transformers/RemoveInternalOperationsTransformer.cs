using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Library.API.Transformers
{
	public class RemoveInternalOperationsTransformer(IHostEnvironment environment) : IOpenApiDocumentTransformer
	{
		public Task TransformAsync(
			OpenApiDocument document,
			OpenApiDocumentTransformerContext context,
			CancellationToken cancellationToken)
		{
			if (!environment.IsProduction())
			{
				return Task.CompletedTask;
			}
			// Remove operations tagged as "Internal"
			foreach (var path in document.Paths.Values)
			{
				var operationsToRemove = path.Operations?
					.Where(op => op.Value.Tags?.Any(tag => tag.Name == "Internal") == true)
					.Select(op => op.Key)
					.ToList() ?? [];
				foreach (var operation in operationsToRemove)
				{
					path.Operations?.Remove(operation);
				}
			}
			// Remove paths that have no operations left
			var pathsToRemove = document.Paths
					.Where(p => p.Value.Operations?.Count == 0)
					.Select(p => p.Key)
					.ToList();

			foreach (var pathToRemove in pathsToRemove)
			{
				document.Paths.Remove(pathToRemove);
			}
			// Remove any tags that have no operations associated with them
			if (document.Tags != null)
			{
				var tagsToRemove = document.Tags
					.Where(tag => !document.Paths.Values
						.Where(p => p.Operations != null)
						.SelectMany(p => p.Operations!.Values)
						.SelectMany(op => op.Tags ?? Enumerable.Empty<OpenApiTagReference>())
						.Any(t => t.Name == tag.Name))
					.ToList();
				foreach (var tag in tagsToRemove)
				{
					document.Tags?.Remove(tag);
				}
			}

			return Task.CompletedTask;
		}
	}
}
