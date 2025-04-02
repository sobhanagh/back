namespace GamaEdtech.Domain.Enumeration
{
    using GamaEdtech.Common.Data.Enumeration;
    using GamaEdtech.Common.DataAnnotation;

    public sealed class ContributionType : Enumeration<ContributionType, byte>
    {
        [Display]
        public static readonly ContributionType School = new(nameof(School), 1);

        [Display]
        public static readonly ContributionType SchoolComment = new(nameof(SchoolComment), 2);

        [Display]
        public static readonly ContributionType SchoolImage = new(nameof(SchoolImage), 3);

        public ContributionType()
        {
        }

        public ContributionType(string name, byte value) : base(name, value)
        {
        }
    }
}
