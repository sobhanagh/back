namespace GamaEdtech.Common.DataAccess.Specification.Impl
{
    using System;
    using System.Linq.Expressions;

    using GamaEdtech.Common.DataAccess.Entities;
    using GamaEdtech.Common.DataAccess.Specification;

    public sealed class CreationUserIdEqualsSpecification<TClass, TKey>(TKey creationUserId) : SpecificationBase<TClass>
        where TClass : class, ICreationUserId<TKey>
        where TKey : IEquatable<TKey>
    {
        public override Expression<Func<TClass, bool>> Expression() => t => t.CreationUserId.Equals(creationUserId);
    }
}
