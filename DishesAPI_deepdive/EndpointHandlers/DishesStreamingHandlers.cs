using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text.Json;
using DishesAPI.DbContexts;
using DishesAPI.Mappers;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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

		public static ServerSentEventsResult<string> GetDishesSseStream(
			DishesDbContext dishesDbContext,
			CancellationToken cancellationToken)
		{
			return TypedResults.ServerSentEvents(
				GenerateDishEvents(dishesDbContext, cancellationToken));
		}

		private static async IAsyncEnumerable<SseItem<string>> GenerateDishEvents(
			DishesDbContext dishesDbContext,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var dishes = await dishesDbContext.Dishes
				.ToListAsync(cancellationToken);

			foreach (var dish in dishes)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var dto = dish.ToDishDto();
				var json = JsonSerializer.Serialize(dto);
				yield return new SseItem<string>(json, "dish");

				// simulate delay between events
				await Task.Delay(500, cancellationToken);

			}

			// Final event signals the end
			yield return new SseItem<string>(
				"stream complete", "done");
		}
	}
}
