namespace GamaEdtech.Backend.Common.DataAccess.Specification.Impl
{
    using System;
    using System.Linq.Expressions;

    using GamaEdtech.Backend.Common.DataAccess.Entities;
    using GamaEdtech.Backend.Common.DataAccess.Specification;

    public sealed class DeletedSpecification<TClass>(bool deleted) : SpecificationBase<TClass>
        where TClass : class, IDeletable<TClass>
    {
        private readonly bool deleted = deleted;

        public override Expression<Func<TClass, bool>> Expression() => t => t.Deleted == deleted;
    }
}
