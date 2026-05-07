using System.ComponentModel.DataAnnotations;

namespace DishesAPI.Models
{
	public class DishForCreationDto
	{
		[Required]
		[StringLength(200, MinimumLength = 2)]
		public required string Name { get; set; }
	}
}
