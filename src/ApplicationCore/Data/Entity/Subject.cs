namespace GamaEdtech.Backend.Data.Entity
{
    using System.Diagnostics.CodeAnalysis;

    using GamaEdtech.Backend.Common.Data;
    using GamaEdtech.Backend.Common.DataAccess.Entities;
    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Common.DataAnnotation.Schema;

    using GamaEdtech.Backend.Data.Entity.Identity;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    [Table(nameof(Subject))]
    public class Subject : VersionableEntity<ApplicationUser, int, int?>, IEntity<Subject, int>
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Int)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [Column(nameof(Title), DataType.UnicodeString)]
        [StringLength(50)]
        [Required]
        public string? Title { get; set; }

        [Column(nameof(Order), DataType.Int)]
        public int Order { get; set; }

        public virtual ICollection<Grade>? Grades { get; set; }
        public virtual ICollection<Topic>? Topics { get; set; }

        public void Configure([NotNull] EntityTypeBuilder<Subject> builder)
        {
            _ = builder.HasMany(t => t.Grades).WithMany(t => t.Subjects)
                .UsingEntity("SubjectGrades",
                j =>
                {
                    _ = j.Property("GradesId").HasColumnName("GradeId");
                    _ = j.Property("SubjectsId").HasColumnName("SubjectId");
                });

            _ = builder.HasMany(t => t.Topics).WithMany(t => t.Subjects)
                .UsingEntity("SubjectTopics",
                j =>
                {
                    _ = j.Property("TopicsId").HasColumnName("TopicId");
                    _ = j.Property("SubjectsId").HasColumnName("SubjectId");
                });
        }
    }
}
