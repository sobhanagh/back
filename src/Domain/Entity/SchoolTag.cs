namespace GamaEdtech.Domain.Entity
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Entities;
    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Common.DataAnnotation.Schema;
    using GamaEdtech.Domain.Entity.Identity;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    [Table(nameof(SchoolTag))]
    public class SchoolTag : IEntity<SchoolTag, long>, ISchoolId, ICreationUser<ApplicationUser, int>
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Long)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [Column(nameof(SchoolId), DataType.Long)]
        [Required]
        public long SchoolId { get; set; }
        public School School { get; set; }

        [Column(nameof(TagId), DataType.Long)]
        [Required]
        public long TagId { get; set; }
        public Tag Tag { get; set; }

        [Column(nameof(CreationUserId), DataType.Int)]
        [Required]
        public int CreationUserId { get; set; }
        public ApplicationUser CreationUser { get; set; }

        [Column(nameof(CreationDate), DataType.DateTimeOffset)]
        [Required]
        public DateTimeOffset CreationDate { get; set; }

        public void Configure([NotNull] EntityTypeBuilder<SchoolTag> builder)
        {
        }
    }
}
