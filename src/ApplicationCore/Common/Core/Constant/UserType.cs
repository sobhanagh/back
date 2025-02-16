namespace GamaEdtech.Backend.Common.Core.Constant
{
    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Common.Resources;

    using GamaEdtech.Backend.Common.Data.Enumeration;

    public sealed class UserType(string name, byte value) : Enumeration<byte>(name, value)
    {
        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(UserType))]
        public static readonly UserType Real = new(nameof(Real), 0);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(UserType))]
        public static readonly UserType Legal = new(nameof(Legal), 1);
    }
}
