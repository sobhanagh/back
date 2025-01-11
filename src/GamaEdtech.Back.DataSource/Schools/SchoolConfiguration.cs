using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GamaEdtech.Back.Domain.Schools;

namespace GamaEdtech.Back.DataSource.Schools;

internal class SchoolConfiguration : IEntityTypeConfiguration<School>
{
	public void Configure(EntityTypeBuilder<School> builder)
	{
		builder.ToTable("School").HasKey(x => x.Id);

		builder.OwnsOne(x => x.Name, b =>
		{
			b.Property(name => name.InEnglish)
				.HasColumnName("NameInEnglish")
				.HasMaxLength(100)
				.IsRequired();

			b.Property(name => name.InLocalLanguage)
				.HasColumnName("NameInLocalLanguage")
				.HasMaxLength(100)
				.IsRequired();
		});

		builder.OwnsOne(x => x.Address, b =>
		{
			b.Property(address => address.Description)
				.HasColumnName("AddressDescription")
				.HasMaxLength(500)
				.IsRequired();

			b.OwnsOne(x => x.Location, b =>
			{
				b.Property(l => l.Geography)
					.HasColumnType("GEOGRAPHY")
					.HasColumnName("AddressGeography")
					.IsRequired(); // Ensures the column is stored as a spatial type
			});

			b.Property(name => name.Country)
				.HasColumnName("AddressCountry")
				.HasMaxLength(50)
				.IsRequired();

			b.Property(name => name.State)
				.HasColumnName("AddressState")
				.HasMaxLength(50)
				.IsRequired();

			b.Property(name => name.City)
				.HasColumnName("AddressCity")
				.HasMaxLength(50)
				.IsRequired();

			b.Property(name => name.ZipCode)
				.HasColumnName("AddressZipCode")
				.HasMaxLength(20)
				.IsRequired();
		});

	}
}
