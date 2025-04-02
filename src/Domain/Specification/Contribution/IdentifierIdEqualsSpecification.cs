namespace GamaEdtech.Domain.Specification.Contribution
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;

    public sealed class IdentifierIdEqualsSpecification(long identifierId) : SpecificationBase<Contribution>
    {
        public override Expression<Func<Contribution, bool>> Expression() => (t) => t.IdentifierId.Equals(identifierId);
    }
}
