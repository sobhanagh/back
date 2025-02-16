namespace GamaEdtech.Backend.Data.ViewModel.Grade
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    public sealed class ManageGradeRequestViewModel
    {
        [Display]
        [Required]
        public string? Title { get; set; }

        [Display]
        public string? Description { get; set; }

        [Display]
        public string? Icon { get; set; }

        [Display]
        [Required]
        public int? BoardId { get; set; }
    }
}
