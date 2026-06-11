using System.Net.NetworkInformation;
using DishesAPI.Entities;
using DishesAPI.Models;
using Riok.Mapperly.Abstractions;

namespace DishesAPI.Mappers
{
	[Mapper]
	public static partial class DishMapper
	{
		[MapperIgnoreSource(nameof(Dish.Ingredients))]
		public static partial DishDto ToDishDto(this Dish dish);

		public static partial IEnumerable<DishDto> ToDishDtoList(this IEnumerable<Dish> dishes);

		[MapperIgnoreTarget(nameof(Dish.Id))]
		[MapperIgnoreTarget(nameof(Dish.Ingredients))]
		public static partial Dish ToDish(this DishForCreationDto dishForCreationDto);

		[MapperIgnoreTarget(nameof(Dish.Id))]
		[MapperIgnoreTarget(nameof(Dish.Ingredients))]

		public static partial void UpdateFromDto(
			this DishForUpdateDto dishForUpdateDto, Dish dish);

		[MapperIgnoreSource(nameof(Ingredient.Dishes))]
		[MapperIgnoreTarget(nameof(IngredientDto.DishId))]
		public static partial IngredientDto MapIngredient(Ingredient ingredient);

		public static IngredientDto ToIngredientDto(
			this Ingredient ingredient, Guid dishId)
		{
			var dto = MapIngredient(ingredient);
			dto.DishId = dishId;
			return dto;

		}

		public static IEnumerable<IngredientDto> ToIngredientDtoList(
			this IEnumerable<Ingredient> ingredients, Guid dishId)
		{
			return ingredients.Select(i => i.ToIngredientDto(dishId));
		}

		public static Dish ToDish(this DishForCreationDto dishForCreationDto)
		{
			return new Dish
			{
				Name = dishForCreationDto.Name
			};
		}

		public static void UpdateFromDto(this Dish dish, DishForUpdateDto dishForUpdateDto)
		{
			dish.Name = dishForUpdateDto.Name;

		}

		[MapperIgnoreSource(nameof(Ingredient.Dishes))]
		[MapperIgnoreTarget(nameof(IngredientDto.DishId))]
		private static partial IngredientDto MapIngredient(Ingredient ingredient);

		public static IngredientDto ToIngredientDto(
			this Ingredient ingredient, Guid dishId
			)
		{
			var dto = MapIngredient(ingredient);
			dto.DishId = dishId;
			return dto;
		}

		public static IEnumerable<IngredientDto> ToIntegrientDtoList(
			this IEnumerable<Ingredient> ingredients, Guid dishId)
		{
			return ingredients.Select(i => i.ToIngredientDto(dishId));
		}

	}
}
