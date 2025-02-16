namespace GamaEdtech.Backend.Data.ViewModel.Identity
{
    using GamaEdtech.Backend.Common.DataAnnotation;

    public class ProfileSettingsRequestViewModel
    {
        [Display]
        [TimeZoneId]
        public string? TimeZoneId { get; set; }
    }
}
