using System.Security.Claims;
using DishesAPI.DbContexts;
using DishesAPI.Mappers;
using DishesAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DishesAPI.EndpointHandlers
{
	public class DishesV2Handlers
	{
		public static async Task<Ok<IEnumerable<DishV2Dto>>> GetDishesAsync(
			DishesDbContext dishesContext,
			ILogger<DishV2Dto> logger,
			ClaimsPrincipal claimsPrincipal,
			string? name
			)
		{
			return TypedResults.Ok((await dishesContext.Dishes
				.Include(d => d.Ingredients)
				.Where(d => name == null || d.Name.Contains(name))
				.ToListAsync())
				.ToDishV2DtoList());

		}

		public static async Task<Results<NotFound, Ok<DishV2Dto>>>
			GetDishByIdAsync(
			DishesDbContext dishesDbContext,
			Guid dishId)
		{
			var dishEntity = await dishesDbContext.Dishes
				.Include(d => d.Ingredients)
				.FirstOrDefaultAsync(d => d.Id == dishId);
			if (dishEntity == null)
			{
				return TypedResults.NotFound();
			}
			return TypedResults.Ok(dishEntity.ToDishV2Dto());
		}
	}
}
