using System.ComponentModel.DataAnnotations;

namespace webapp.Validators;

sealed public class FutureDateAttribute : ValidationAttribute {
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value == null) return ValidationResult.Success;

        if (value is not DateTime dateValue)
            return new ValidationResult("O valor informado não é uma Data válida");
        
        return dateValue.Date <= DateTime.Today
            ? new ValidationResult(ErrorMessage)
            : ValidationResult.Success;
    }
}