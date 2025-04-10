namespace GamaEdtech.Domain.Enumeration
{
    using GamaEdtech.Common.Data.Enumeration;
    using GamaEdtech.Common.DataAnnotation;

    public sealed class ContributionType : Enumeration<ContributionType, byte>
    {
        [Display]
        public static readonly ContributionType School = new(nameof(School), 1, "SchoolContributionPoints");

        [Display]
        public static readonly ContributionType SchoolComment = new(nameof(SchoolComment), 2, "SchoolImageContributionPoints");

        [Display]
        public static readonly ContributionType SchoolImage = new(nameof(SchoolImage), 3, "SchoolCommentContributionPoints");

        public string? ApplicationSettingsName { get; set; }

        public ContributionType()
        {
        }

        public ContributionType(string name, byte value, string applicationSettingsName)
            : base(name, value) => ApplicationSettingsName = applicationSettingsName;
    }
}
