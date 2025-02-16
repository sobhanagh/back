namespace GamaEdtech.Backend.Common.Data
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class SearchFilter
    {
        [Required]
        public string? Phrase { get; set; }

        [Required]
        public string? Column { get; set; }
    }
}
