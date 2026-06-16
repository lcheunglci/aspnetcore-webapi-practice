using System.Runtime.CompilerServices;
using DishesAPI.DbContexts;
using DishesAPI.Mappers;
using DishesAPI.Models;

namespace DishesAPI.EndpointHandlers
{
	public class DishesStreamingHandlers
	{
		public static async IAsyncEnumerable<DishDto> GetDishesStreamAsync(
			DishesDbContext dishesDbContext,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await foreach (var dish in dishesDbContext.Dishes
				.AsAsyncEnumerable()
				.WithCancellation(cancellationToken))
			{
				yield return dish.ToDishDto();
			}
		} 
	}
}
