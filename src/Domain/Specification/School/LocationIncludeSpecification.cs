namespace GamaEdtech.Domain.Specification.School
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;

    public sealed class LocationIncludeSpecification(double latitude, double longitude, double radius) : SpecificationBase<School>
    {
        public override Expression<Func<School, bool>> Expression()
        {
            latitude += radius;
            longitude += radius;
            return (t) => t.Latitude >= latitude && t.Longitude <= longitude;
        }
    }
}
