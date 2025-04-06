namespace GamaEdtech.Domain.Enumeration
{
    using GamaEdtech.Common.Data.Enumeration;
    using GamaEdtech.Common.DataAnnotation;

    using System.Collections;

    public sealed class SystemClaim : FlagsEnumeration<SystemClaim>
    {
        [Display]
        public static readonly SystemClaim AutoConfirmSchoolContribution = new(1);

        public SystemClaim()
        {
        }

        public SystemClaim(BitArray value)
            : base(value)
        {
        }

        public SystemClaim(byte[] value)
            : base(value)
        {
        }

        public SystemClaim(int index)
            : base(index)
        {
        }
    }
}
