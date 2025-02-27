using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace webapp.Validators;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
sealed public class IntervalValidation : ValidationAttribute {
    private readonly string _beginField;
    private readonly string _endField;

    public IntervalValidation(string beginField, string endField, string ErrorMessage) : base(ErrorMessage){
        _beginField = beginField;
        _endField = endField;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext) {
        if (value == null)
            return ValidationResult.Success;

        /*
            We use the object, because sometimes the context is passed
            incorrectly by Razor, resulting in wrong error messages.
        */
        var type = value.GetType();

        var beginProperty = type.GetProperty(_beginField);
        var endProperty = type.GetProperty(_endField);

        if(beginProperty == null || endProperty == null)
            return new ValidationResult($"The {_beginField} or {_endField} don't exist.");

        var valueBeginDate = beginProperty.GetValue(value) as DateTime?;
        var valueEndDate = endProperty.GetValue(value) as DateTime?;

        if(valueBeginDate == null || valueEndDate == null)
            return new ValidationResult("The begin and end dates must be defined.");

        if(valueEndDate < valueBeginDate)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}