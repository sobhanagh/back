namespace GamaEdtech.Domain.Specification.School
{
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Specification;
    using GamaEdtech.Domain.Entity;

    public sealed class BoardIdEqualsSpecification(int boardId) : SpecificationBase<School>
    {
        //public override Expression<Func<School, bool>> Expression() => (t) => t.boardId == boardId
        public override Expression<Func<School, bool>> Expression() => (t) => boardId > 0;
    }
}
