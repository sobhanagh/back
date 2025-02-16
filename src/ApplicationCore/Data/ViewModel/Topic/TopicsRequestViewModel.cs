namespace GamaEdtech.Backend.Data.ViewModel.Topic
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class TopicsRequestViewModel
    {
        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new(), };

        [Display]
        public int? SubjectId { get; set; }
    }
}
