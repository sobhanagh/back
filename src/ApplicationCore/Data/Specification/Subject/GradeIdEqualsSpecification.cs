namespace GamaEdtech.Backend.Data.Specification.Subject
{
    using System.Linq.Expressions;

    using GamaEdtech.Backend.Common.DataAccess.Specification;

    using GamaEdtech.Backend.Data.Entity;

    public sealed class GradeIdEqualsSpecification(int gradeId) : SpecificationBase<Subject>
    {
        public override Expression<Func<Subject, bool>> Expression() => (t) => t.Grades!.Any(g => g.Id == gradeId);
    }
}
