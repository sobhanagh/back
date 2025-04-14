namespace GamaEdtech.Common.DataAccess.Entities
{
    using System;

    using Microsoft.AspNetCore.Identity;

    public interface IVersionableEntity<TUser, TKey, TLastModifyKey> : ICreationUser<TUser, TKey>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        DateTimeOffset? LastModifyDate { get; set; }

        TLastModifyKey? LastModifyUserId { get; set; }

        TUser? LastModifyUser { get; set; }
    }
}
