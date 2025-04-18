namespace GamaEdtech.Presentation.ViewModel.Blog
{
    using GamaEdtech.Common.DataAnnotation;

    public sealed class ManagePostRequestViewModel
    {
        [Display]
        [Required]
        public string? Title { get; set; }

        [Display]
        public string? Description { get; set; }

        [Display]
        public string? Icon { get; set; }
    }
}
