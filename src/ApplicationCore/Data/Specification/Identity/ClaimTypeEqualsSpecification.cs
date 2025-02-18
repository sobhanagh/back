namespace GamaEdtech.Data.Specification.Identity
{
    using GamaEdtech.Data.Entity.Identity;

    using GamaEdtech.Common.DataAccess.Specification;

    using System.Linq.Expressions;

    public sealed class ClaimTypeEqualsSpecification(string claimType) : SpecificationBase<ApplicationUserClaim>
    {
        public override Expression<Func<ApplicationUserClaim, bool>> Expression() => (t) => t.ClaimType == claimType;
    }
}
