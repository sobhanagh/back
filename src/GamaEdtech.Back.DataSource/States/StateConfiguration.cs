using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using GamaEdtech.Back.Domain.States;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Domain.Base;

namespace GamaEdtech.Back.DataSource.Schools;

internal class StateConfiguration : IEntityTypeConfiguration<State>
{
	public void Configure(EntityTypeBuilder<State> builder)
	{
		builder.ToTable("State").HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasConversion(
				id => id.Value,
				value => new Id(value)
			)
			.HasColumnName("Id")
			.ValueGeneratedOnAdd();

		builder.Property(x => x.Name)
			.HasColumnName("Name")
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(x => x.Code)
			.HasColumnName("Code")
			.HasMaxLength(5)
			.IsRequired();

		builder.HasOne<Country>()
			.WithMany()
			.HasForeignKey(x => x.CountryId);

	}
}
