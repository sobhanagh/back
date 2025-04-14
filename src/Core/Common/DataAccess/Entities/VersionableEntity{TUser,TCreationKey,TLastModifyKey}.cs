namespace GamaEdtech.Common.DataAccess.Entities
{
    using System;

    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Common.DataAnnotation.Schema;

    using Microsoft.AspNetCore.Identity;

    public abstract class VersionableEntity<TUser, TCreationKey, TLastModifyKey> : IVersionableEntity<TUser, TCreationKey, TLastModifyKey>
        where TUser : IdentityUser<TCreationKey>
        where TCreationKey : IEquatable<TCreationKey>
    {
        [Column(nameof(CreationDate), Data.DataType.DateTimeOffset)]
        [Required]
        public virtual DateTimeOffset CreationDate { get; set; }

        [Column(nameof(CreationUserId))]
        [Required]
        public virtual TCreationKey CreationUserId { get; set; }
        public TUser CreationUser { get; set; }

        [Column(nameof(LastModifyDate), Data.DataType.DateTimeOffset)]
        public virtual DateTimeOffset? LastModifyDate { get; set; }

        [Column(nameof(LastModifyUserId))]
        public virtual TLastModifyKey? LastModifyUserId { get; set; }

        public TUser? LastModifyUser { get; set; }
    }
}
