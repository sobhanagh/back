using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Schools;
using Microsoft.EntityFrameworkCore;

namespace GamaEdtech.Back.DataSource.Schools;

public class SqlServerSchoolRepository : ISchoolRepository
{
	private readonly GamaEdtechDbContext _dbContext;

	public SqlServerSchoolRepository(GamaEdtechDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<IReadOnlyList<School>> Find(
		SchoolName? name = null, 
		Location? location = null)
	{
		IQueryable<School> schools = _dbContext.Schools;

		if (name is not null)
			schools = schools.Where(x => x.Name == name);


		if (location is not null)
			schools = schools.Where(x => x.Address.Location == location);

		return await schools.ToListAsync();

	}

	public async Task Add(School school)
	{
		await _dbContext.Schools.AddAsync(school);
	}
}
