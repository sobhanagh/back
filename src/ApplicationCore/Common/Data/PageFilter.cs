namespace GamaEdtech.Backend.Common.Data
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class PageFilter
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Skip { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int Size { get; set; } = 1;

        public ExportType? ExportType { get; set; }

        public bool ReturnTotalRecordsCount { get; set; }
    }
}
