namespace GamaEdtech.Presentation.ViewModel.School
{
    using GamaEdtech.Common.DataAnnotation;

    public sealed class ManageSchoolContributionRequestViewModel : ManageNewSchoolContributionRequestViewModel
    {
        [Required(false)]
        public override string? Name { get; set; }

        [Required(false)]
        public override double? Latitude { get; set; }

        [Required(false)]
        public override double? Longitude { get; set; }

        public long? DefaultImageId { get; set; }
    }
}
