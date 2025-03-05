namespace GamaEdtech.Presentation.ViewModel.School
{
    using GamaEdtech.Common.DataAnnotation;

    public sealed class ManageSchoolCommentRequestViewModel
    {
        [Display]
        [Required]
        public string? Comment { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte ClassesQualityRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte EducationRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte ITTrainingRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte SafetyAndHappinessRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte BehaviorRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte TuitionRatioRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte FacilitiesRate { get; set; }

        [Display]
        [Range(typeof(byte), "0", "5")]
        public byte ArtisticActivitiesRate { get; set; }
    }
}
