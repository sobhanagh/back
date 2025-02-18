namespace GamaEdtech.Data.Specification.School
{
    using System.Linq;
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;

    using GamaEdtech.Data.Entity;

    public sealed class CityIdsContainsSpecification(IEnumerable<int> cityIds) : SpecificationBase<School>
    {
        public override Expression<Func<School, bool>> Expression() => (t) => t.CityId.HasValue && cityIds.Contains(t.CityId.Value);
    }
}
