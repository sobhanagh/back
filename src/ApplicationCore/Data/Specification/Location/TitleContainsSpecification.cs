namespace GamaEdtech.Backend.Data.Specification.Location
{
    using System.Linq.Expressions;

    using GamaEdtech.Backend.Common.DataAccess.Specification;

    using GamaEdtech.Backend.Data.Entity;

    public sealed class TitleContainsSpecification(string title) : SpecificationBase<Location>
    {
        public override Expression<Func<Location, bool>> Expression() => (t) => t.Title!.Contains(title);
    }
}
