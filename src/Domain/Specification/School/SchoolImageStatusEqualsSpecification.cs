namespace GamaEdtech.Domain.Specification.School
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;

    public sealed class SchoolImageStatusEqualsSpecification(Status status) : SpecificationBase<SchoolImage>
    {
        public override Expression<Func<SchoolImage, bool>> Expression() => (t) => t.Status == status;
    }
}
