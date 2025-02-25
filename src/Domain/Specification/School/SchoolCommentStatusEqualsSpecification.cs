namespace GamaEdtech.Domain.Specification.School
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;
    using GamaEdtech.Domain.Enumeration;

    public sealed class SchoolCommentStatusEqualsSpecification(Status status) : SpecificationBase<SchoolComment>
    {
        public override Expression<Func<SchoolComment, bool>> Expression() => (t) => t.Status == status;
    }
}
