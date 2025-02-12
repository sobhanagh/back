namespace GamaEdtech.Backend.Data.Specification.School
{
    using System.Linq;
    using System.Linq.Expressions;

    using Farsica.Framework.DataAccess.Specification;

    using GamaEdtech.Backend.Data.Entity;

    public sealed class CityIdsContainsSpecification(IEnumerable<int> cityIds) : SpecificationBase<School>
    {
        public override Expression<Func<School, bool>> Expression() => (t) => t.CityId.HasValue && cityIds.Contains(t.CityId.Value);
    }
}
