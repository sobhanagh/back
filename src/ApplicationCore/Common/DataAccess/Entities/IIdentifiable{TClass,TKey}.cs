namespace GamaEdtech.Backend.Common.DataAccess.Entities
{
    using System;

    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    public interface IIdentifiable<TClass, TKey>
        where TClass : class
        where TKey : IEquatable<TKey>
    {
        [NotMapped]
        TKey Id { get; set; }
    }
}
