namespace GamaEdtech.Domain.Entity
{
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Common.Data;
    using GamaEdtech.Common.DataAccess.Entities;
    using GamaEdtech.Common.DataAnnotation;
    using GamaEdtech.Common.DataAnnotation.Schema;
    using GamaEdtech.Domain.Entity.Identity;

    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    [Table(nameof(Reward))]
    public class Reward : IEntity<Reward, long>
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Int)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public long Id { get; set; }

        [Column(nameof(UserId), DataType.Int)]
        [Required]
        public int UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Column(nameof(ContributionId), DataType.Long)]
        [Required]
        public long ContributionId { get; set; }
        public Contribution? Contribution { get; set; }

        [Column(nameof(Points), DataType.Int)]
        [Required]
        public int Points { get; set; }

        [Column(nameof(Claimed), DataType.Boolean)]
        public bool Claimed { get; set; }

        [Column(nameof(ClaimedDate), DataType.DateTimeOffset)]
        public DateTimeOffset? ClaimedDate { get; set; }

        public void Configure([NotNull] EntityTypeBuilder<Reward> builder)
        {
        }
    }
}
