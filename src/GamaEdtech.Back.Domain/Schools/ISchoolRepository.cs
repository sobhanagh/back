namespace GamaEdtech.Back.Domain.Schools;

public interface ISchoolRepository
{
	public Task<IReadOnlyList<School>> Find(
		SchoolName? name = null, 
		Location? location = null);

	public Task Add(School school);
}
