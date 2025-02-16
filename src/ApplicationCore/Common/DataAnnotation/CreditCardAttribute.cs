namespace GamaEdtech.Backend.Common.DataAnnotation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using GamaEdtech.Backend.Common.Core;

    using GamaEdtech.Backend.Common.Core.Extensions.Collections.Generic;

    using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

    public sealed class CreditCardAttribute : ValidationAttribute, IClientModelValidator
    {
        public void AddValidation([NotNull] ClientModelValidationContext context)
        {
            _ = context.Attributes.AddIfNotContains(new KeyValuePair<string, string>("data-val", "true"));

            var msg = FormatErrorMessage(Globals.GetLocalizedDisplayName(context.ModelMetadata.ContainerType?.GetProperty(context.ModelMetadata.Name)));
            _ = context.Attributes.AddIfNotContains(new KeyValuePair<string, string>("data-val-creditcard", Data.Error.FormatMessage(msg)));
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return ValidationResult.Success;
            }

            var attribute = new System.ComponentModel.DataAnnotations.CreditCardAttribute();
            return value is List<string> lst
                ? lst.All(t => string.IsNullOrEmpty(t) || attribute.IsValid(t)) ? ValidationResult.Success : new ValidationResult(ErrorMessage)
                : attribute.IsValid(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
