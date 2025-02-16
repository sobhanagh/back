namespace GamaEdtech.Backend.Common.DataAnnotation
{
    using System.Collections.Generic;
    using System.Linq;

    using GamaEdtech.Backend.Common.Core;

    public sealed class NationalIdAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => string.IsNullOrEmpty(value?.ToString())
|| (value is IEnumerable<string> lst
                ? lst.All(t => string.IsNullOrEmpty(t) || Globals.ValidateNationalId(t))
                : Globals.ValidateNationalId(value.ToString()!));
    }
}
