using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models;

public class CourseForCreationDto
{
	[Required(ErrorMessage = "You should fill out a title.")]
	[MaxLength(100, ErrorMessage = "The title should not have more than 100 characters.")]
    public string Title { get; set; } = string.Empty;
	[MaxLength(1000, ErrorMessage = "The description should not have more than 1000 characters.")]
    public string? Description { get; set; } = string.Empty;
}
