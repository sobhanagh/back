namespace GamaEdtech.Presentation.ViewModel.Contact
{
    using GamaEdtech.Common.DataAnnotation;

    public sealed class ManageContactRequestViewModel
    {
        [Display]
        [Required]
        public string? Captcha { get; set; }

        [Display]
        [Required]
        public string? FullName { get; set; }

        [Display]
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Display]
        [Required]
        public string? Subject { get; set; }

        [Display]
        [Required]
        public string? Body { get; set; }
    }
}
