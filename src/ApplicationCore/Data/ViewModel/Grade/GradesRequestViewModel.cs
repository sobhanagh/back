namespace GamaEdtech.Backend.Data.ViewModel.Grade
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class GradesRequestViewModel
    {
        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new(), };

        [Display]
        public int? BoardId { get; set; }
    }
}
