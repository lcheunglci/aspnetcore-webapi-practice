namespace DishesAPI.Models
{
	public class DishV2Dto
	{
		public Guid Id { get; set; }
		public required string Name { get; set; }
		public int IngredientCount { get; set; }
	}
}
