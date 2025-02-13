namespace GamaEdtech.Backend.Data.Entity
{
    using System.Diagnostics.CodeAnalysis;

    using Farsica.Framework.Data;
    using Farsica.Framework.Data.Enumeration;
    using Farsica.Framework.DataAccess.Entities;
    using Farsica.Framework.DataAnnotation;
    using Farsica.Framework.DataAnnotation.Schema;

    using GamaEdtech.Backend.Data.Entity.Identity;
    using GamaEdtech.Backend.Data.Enumeration;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    [Table(nameof(School))]
    public class School : VersionableEntity<ApplicationUser, int, int?>, IEntity<School, int>
    {
        [System.ComponentModel.DataAnnotations.Key]
        [Column(nameof(Id), DataType.Int)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [Column(nameof(OsmId), DataType.Long)]
        public long? OsmId { get; set; }

        [Column(nameof(Name), DataType.UnicodeString)]
        [StringLength(300)]
        [Required]
        public string? Name { get; set; }

        [Column(nameof(LocalName), DataType.UnicodeString)]
        [StringLength(300)]
        public string? LocalName { get; set; }

        [Column(nameof(SchoolType), DataType.Byte)]
        [Required]
        public SchoolType? SchoolType { get; set; }

        [Column(nameof(CountryId), DataType.Int)]
        public int? CountryId { get; set; }
        public Location? Country { get; set; }

        [Column(nameof(StateId), DataType.Int)]
        public int? StateId { get; set; }
        public Location? State { get; set; }

        [Column(nameof(CityId), DataType.Int)]
        public int? CityId { get; set; }
        public Location? City { get; set; }

        [Column(nameof(ZipCode), DataType.String)]
        [StringLength(100)]
        public string? ZipCode { get; set; }

        [Column(nameof(Address), DataType.UnicodeMaxString)]
        public string? Address { get; set; }

        [Column(nameof(LocalAddress), DataType.UnicodeMaxString)]
        public string? LocalAddress { get; set; }

        [Column(nameof(Latitude), TypeName = "float")]
        public double? Latitude { get; set; }

        [Column(nameof(Longitude), TypeName = "float")]
        public double? Longitude { get; set; }

        [Column(nameof(Quarter), DataType.UnicodeString)]
        [StringLength(100)]
        public string? Quarter { get; set; }

        [Column(nameof(FaxNumber), DataType.UnicodeString)]
        [StringLength(200)]
        public string? FaxNumber { get; set; }

        [Column(nameof(PhoneNumber), DataType.UnicodeString)]
        [StringLength(200)]
        public string? PhoneNumber { get; set; }

        [Column(nameof(Email), DataType.UnicodeString)]
        [StringLength(200)]
        public string? Email { get; set; }

        [Column(nameof(WebSite), DataType.UnicodeString)]
        [StringLength(300)]
        public string? WebSite { get; set; }

        [Column(nameof(Facilities), DataType.UnicodeMaxString)]
        public string? Facilities { get; set; }

        public void Configure([NotNull] EntityTypeBuilder<School> builder)
        {
            _ = builder.OwnEnumeration<School, SchoolType, byte>(t => t.SchoolType);
            _ = builder.HasOne(t => t.Country).WithMany().HasForeignKey(t => t.CountryId).OnDelete(DeleteBehavior.NoAction);
            _ = builder.HasOne(t => t.State).WithMany().HasForeignKey(t => t.StateId).OnDelete(DeleteBehavior.NoAction);
            _ = builder.HasOne(t => t.City).WithMany().HasForeignKey(t => t.CityId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
