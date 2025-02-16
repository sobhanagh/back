namespace GamaEdtech.Backend.Common.Data
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    using static GamaEdtech.Backend.Common.Core.Constants;

    public sealed class SortFilter
    {
        public SortType SortType { get; set; }

        [Required]
        public string? Column { get; set; }
    }
}
