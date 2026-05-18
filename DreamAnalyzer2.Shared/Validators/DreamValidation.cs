using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DreamAnalyzer2.Shared.Validators
{
    public static class DreamValidation
    {
        public static ValidationResult? ValidateDate(DateTime date, ValidationContext validationContext)
        {
            if (date > DateTime.UtcNow)
                return new ValidationResult("Dream date can't be in the future");
            return ValidationResult.Success;
        }
    }
}
