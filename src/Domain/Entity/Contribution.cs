namespace GamaEdtech.Domain.Entity
{
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.Data.Enumeration;
    using GamaEdtech.Common.DataAccess.Entities;
    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Common.DataAnnotation.Schema;
    using GamaEdtech.Domain.Entity.Identity;
    using GamaEdtech.Domain.Enumeration;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    [Table(nameof(Contribution))]
    public class Contribution : VersionableEntity<ApplicationUser, int, int?>, IStatus, IEntity<Contribution, long>
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Long)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [Column(nameof(ContributionType), DataType.Byte)]
        [Required]
        public ContributionType? ContributionType { get; set; }

        [Column(nameof(Status), DataType.Byte)]
        [Required]
        public Status? Status { get; set; }

        [Column(nameof(Comment), DataType.UnicodeString)]
        [StringLength(300)]
        public string? Comment { get; set; }

        [Column(nameof(Data), DataType.UnicodeMaxString)]
        public string? Data { get; set; }

        [Column(nameof(IdentifierId), DataType.Long)]
        public long? IdentifierId { get; set; }

        public void Configure([NotNull] EntityTypeBuilder<Contribution> builder)
        {
            _ = builder.OwnEnumeration<Contribution, ContributionType, byte>(t => t.ContributionType);
            _ = builder.OwnEnumeration<Contribution, Status, byte>(t => t.Status);
            _ = builder.HasIndex(t => t.Status);
        }
    }
}
