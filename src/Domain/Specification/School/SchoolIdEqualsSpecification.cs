namespace GamaEdtech.Domain.Specification.School
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;

    public sealed class SchoolIdEqualsSpecification(int schoolId) : SpecificationBase<SchoolComment>
    {
        public override Expression<Func<SchoolComment, bool>> Expression() => (t) => t.SchoolId == schoolId;
    }
}
