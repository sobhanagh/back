namespace GamaEdtech.Domain.Specification.Contribution
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;

    public sealed class ContributionTypeEqualsSpecification(ContributionType contributionType) : SpecificationBase<Contribution>
    {
        public override Expression<Func<Contribution, bool>> Expression() => (t) => t.ContributionType!.Equals(contributionType);
    }
}
