namespace GamaEdtech.Backend.Common.Core.Constant
{
    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Common.Resources;

    using GamaEdtech.Backend.Common.Data.Enumeration;

    public sealed class DegreeType(string name, byte value) : Enumeration<byte>(name, value)
    {
        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType Diploma = new(nameof(Diploma), 0);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType Associate = new(nameof(Associate), 1);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType Bachelor = new(nameof(Bachelor), 2);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType Master = new(nameof(Master), 3);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType Phd = new(nameof(Phd), 4);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType PostDoc = new(nameof(PostDoc), 5);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(DegreeType))]
        public static readonly DegreeType Other = new(nameof(Other), 6);
    }
}
