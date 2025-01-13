using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GamaEdtech.Back.Domain.Base;
using GamaEdtech.Back.Domain.Cities;
using GamaEdtech.Back.Domain.States;
using GamaEdtech.Back.Domain.Countries;

namespace GamaEdtech.Back.DataSource.Contries;

internal class CityConfiguration : IEntityTypeConfiguration<City>
{
	public void Configure(EntityTypeBuilder<City> builder)
	{
		builder.ToTable("City").HasKey(x => x.Id);

		builder.Property(x => x.Id)
		.HasConversion(
			id => id.Value,   
			value => new Id(value)
		)
		.HasColumnName("Id")
		.ValueGeneratedOnAdd();

		builder.Property(x => x.Name)
			.HasColumnName("Name")
			.HasMaxLength(100)
			.IsRequired();

		builder.HasOne<State>()
			.WithMany()
			.HasForeignKey(x => x.StateId)
			.IsRequired(false);

		builder.HasOne<Country>()
			.WithMany()
			.HasForeignKey(x => x.CountryId)
			.IsRequired();
	}
}

