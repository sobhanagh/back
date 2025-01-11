using GamaEdtech.Back.Domain.Schools;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GamaEdtech.Back.Domain.Countries;


namespace GamaEdtech.Back.DataSource.Contries;

internal class CountryConfiguration : IEntityTypeConfiguration<Country>
{
	public void Configure(EntityTypeBuilder<Country> builder)
	{
		builder.ToTable("Country").HasKey(x => x.Id);

		builder.Property(x => x.Name)
				.HasColumnName("Name")
				.HasMaxLength(50)
				.IsRequired();

		builder.Property(x => x.Code)
				.HasColumnName("Code")
				.HasMaxLength(3)
				.IsRequired();
	}
}

