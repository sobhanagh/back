namespace GamaEdtech.Backend.Common.DataAnnotation
{
    public sealed class DisplayFormatAttribute : System.ComponentModel.DataAnnotations.DisplayFormatAttribute
    {
        public Core.Constants.FormatProvider FormatProvider { get; set; }
    }
}
