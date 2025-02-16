namespace GamaEdtech.Backend.Common.DataAccess.Specification
{
    using System;
    using System.Linq.Expressions;

    using GamaEdtech.Backend.Common.Core.Extensions.Linq;

    public class NotSpecification<T>(ISpecification<T> left) : SpecificationBase<T>
    {
        private readonly ISpecification<T> left = left;

        public override Expression<Func<T, bool>> Expression() => left.Expression().Not();
    }
}
