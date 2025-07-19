namespace GamaEdtech.Presentation.ViewModel.Identity
{
    using GamaEdtech.Common.DataAnnotation;

    public class ProfileSettingsRequestViewModel
    {
        [Display]
        [TimeZoneId]
        public string? TimeZoneId { get; set; }
        public string? CountryId { get; set; }
        public string? SchoolId { get; set; }
        public string? StateId { get; set; }
    }
}
