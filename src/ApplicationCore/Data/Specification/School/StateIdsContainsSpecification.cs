namespace GamaEdtech.Backend.Data.Specification.School
{
    using System.Linq;
    using System.Linq.Expressions;

    using Farsica.Framework.DataAccess.Specification;

    using GamaEdtech.Backend.Data.Entity;

    public sealed class StateIdsContainsSpecification(IEnumerable<int> stateIds) : SpecificationBase<School>
    {
        public override Expression<Func<School, bool>> Expression() => (t) => t.StateId.HasValue && stateIds.Contains(t.StateId.Value);
    }
}
