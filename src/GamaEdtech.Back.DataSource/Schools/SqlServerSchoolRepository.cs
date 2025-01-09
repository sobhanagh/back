using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Schools;

namespace GamaEdtech.Back.DataSource.Schools;

public class SqlServerSchoolRepository : ISchoolRepository
{
	private readonly GamaEdtechDbContext _dbContext;

	public SqlServerSchoolRepository(GamaEdtechDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task Add(School school)
	{
		await _dbContext.Schools.AddAsync(school);
	}
}
