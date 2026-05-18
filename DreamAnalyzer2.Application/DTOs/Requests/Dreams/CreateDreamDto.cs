using DreamAnalyzer2.Shared.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamAnalyzer2.Application.DTOs.Requests.Dreams
{
    public class CreateDreamDto
    {
        [Required(ErrorMessage = "Dream title is required")]
        [MinLength(3, ErrorMessage = "Dream title must be at least 3 characters long"), MaxLength(50, ErrorMessage = "Dream title must be less than or equal to 50 characters long")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dream content is required")]
        [MinLength(3, ErrorMessage = "Dream content must be at least 3 characters long"), MaxLength(3000, ErrorMessage = "Dream content must be less than or equal to 3000 characters long")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dream date is required")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(DreamValidation), nameof(DreamValidation.ValidateDate))]
        public DateTime DreamDate { get; set; }
    }
}
