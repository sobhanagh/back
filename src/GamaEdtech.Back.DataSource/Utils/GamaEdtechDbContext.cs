using GamaEdtech.Back.DataSource.Schools;
using GamaEdtech.Back.Domain.Schools;
using Microsoft.EntityFrameworkCore;

namespace GamaEdtech.Back.DataSource.Utils;

public class GamaEdtechDbContext : DbContext
{
	private readonly ConnectionString _connectionString;

	public GamaEdtechDbContext(ConnectionString connectionString)
	{
		_connectionString = connectionString;
	}

	public DbSet<School> Schools { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		base.OnConfiguring(optionsBuilder);
		optionsBuilder
			//.UseLazyLoadingProxies()
			.UseSqlServer(
			_connectionString.Value,
			x => x.UseNetTopologySuite());
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		ApplyConfigurations(modelBuilder);
	}

	private static void ApplyConfigurations(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new SchoolConfiguration());
	}
}

