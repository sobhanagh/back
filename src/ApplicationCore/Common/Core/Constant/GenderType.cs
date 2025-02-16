namespace GamaEdtech.Backend.Common.Core.Constant
{
    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Common.Resources;

    using GamaEdtech.Backend.Common.Data.Enumeration;

    public sealed class GenderType(string name, byte value) : Enumeration<byte>(name, value)
    {
        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(GenderType))]
        public static readonly GenderType Male = new(nameof(Male), 0);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(GenderType))]
        public static readonly GenderType Female = new(nameof(Female), 1);
    }
}
