using System.ComponentModel.DataAnnotations;

namespace webapp.Validators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class IntervalValidation : ValidationAttribute {
    private readonly string _beginField;
    private readonly string _endField;

    public IntervalValidation(string beginField, string endField) {
        _beginField = beginField;
        _endField = endField;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
        if (value == null)
            return ValidationResult.Success;

        var type = validationContext.ObjectType;

        var beginProperty = type.GetProperty(_beginField);
        var endProperty = type.GetProperty(_endField);

        if(beginProperty == null || endProperty == null)
            return new ValidationResult($"The {_beginField} or {_endField} don't exist.");

        var valueBeginDate = beginProperty.GetValue(validationContext.ObjectInstance) as DateTime?;
        var valueEndDate = endProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

        if(valueBeginDate == null || valueEndDate == null)
            return new ValidationResult("The begin and end dates must be defined.");

        if(valueEndDate < valueBeginDate)
            return new ValidationResult("The end date must be greater than the end date.");

        return ValidationResult.Success;
    }
}