namespace GamaEdtech.Backend.Data.ViewModel.Subject
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class SubjectsRequestViewModel
    {
        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new(), };

        [Display]
        public int? GradeId { get; set; }
    }
}
