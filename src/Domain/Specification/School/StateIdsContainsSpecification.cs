namespace GamaEdtech.Domain.Specification.School
{
    using System.Linq;
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;

    public sealed class StateIdsContainsSpecification(IEnumerable<int> stateIds) : SpecificationBase<School>
    {
        public override Expression<Func<School, bool>> Expression() => (t) => t.StateId.HasValue && stateIds.Contains(t.StateId.Value);
    }
}
