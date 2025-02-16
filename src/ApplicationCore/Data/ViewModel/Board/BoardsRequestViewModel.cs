namespace GamaEdtech.Backend.Data.ViewModel.Board
{
    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class BoardsRequestViewModel
    {
        [Display]
        public PagingDto? PagingDto { get; set; } = new() { PageFilter = new(), };
    }
}
