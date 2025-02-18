namespace GamaEdtech.Data.Specification.Location
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;

    using GamaEdtech.Data.Entity;
    using GamaEdtech.Data.Enumeration;

    public sealed class LocationTypeEqualsSpecification(LocationType locationType) : SpecificationBase<Location>
    {
        public override Expression<Func<Location, bool>> Expression() => (t) => t.LocationType == locationType;
    }
}
