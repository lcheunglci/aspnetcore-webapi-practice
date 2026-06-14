using System.ComponentModel.DataAnnotations;

namespace DishesAPI.Models;

public class DishForCreationDto : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public required string Name { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (Name.Contains("test", StringComparison.OrdinalIgnoreCase))
		{
			yield return new ValidationResult("Name cannot contain the word 'test'", new[] { nameof(Name) });
		}
	}
}
