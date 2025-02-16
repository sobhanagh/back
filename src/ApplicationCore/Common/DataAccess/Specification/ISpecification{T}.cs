namespace GamaEdtech.Backend.Common.DataAccess.Specification
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using GamaEdtech.Backend.Common.Data;

    public interface ISpecification<T>
    {
        PageFilter? PageFilter { get; }

        Func<IQueryable<T>, IOrderedQueryable<T>>? Order { get; }

        Expression<Func<T, bool>> Expression();

        bool IsSatisfiedBy(T candidate);
    }
}
