namespace GamaEdtech.Backend.Common.DataAccess.Specification.Impl
{
    using System;
    using System.Linq.Expressions;

    using GamaEdtech.Backend.Common.DataAccess.Entities;
    using GamaEdtech.Backend.Common.DataAccess.Specification;

    public sealed class IdEqualsSpecification<TClass, TKey>(TKey id) : SpecificationBase<TClass>
        where TClass : class, IIdentifiable<TClass, TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly TKey id = id;

        public override Expression<Func<TClass, bool>> Expression() => t => t.Id.Equals(id);
    }
}
