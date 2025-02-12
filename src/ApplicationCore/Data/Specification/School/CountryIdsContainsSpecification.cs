namespace GamaEdtech.Backend.Data.Specification.School
{
    using System.Linq;
    using System.Linq.Expressions;

    using Farsica.Framework.DataAccess.Specification;

    using GamaEdtech.Backend.Data.Entity;

    public sealed class CountryIdsContainsSpecification(IEnumerable<int> countryIds) : SpecificationBase<School>
    {
        public override Expression<Func<School, bool>> Expression() => (t) => t.CountryId.HasValue && countryIds.Contains(t.CountryId.Value);
    }
}
