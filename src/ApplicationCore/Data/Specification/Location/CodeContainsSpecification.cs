namespace GamaEdtech.Data.Specification.Location
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;

    using GamaEdtech.Data.Entity;

    public sealed class CodeContainsSpecification(string code) : SpecificationBase<Location>
    {
        public override Expression<Func<Location, bool>> Expression() => (t) => t.Code!.Contains(code);
    }
}
