﻿using CourseLibrary.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Services
{
    public class CourseTitleMustBeDifferentFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var course = (CourseForManipulationDto)validationContext.ObjectInstance;

            if (course.Title == course.Description)
            {
                return new ValidationResult(
                    ErrorMessage,
                    new[] { nameof(CourseForManipulationDto) });
            }

            return ValidationResult.Success;
        }
    }
}
