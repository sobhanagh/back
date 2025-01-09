using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Schools;
using Microsoft.AspNetCore.Mvc;

namespace GamaEdtech.Back.Gateway.Rest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SchoolsController : ControllerBase
{
	private readonly GamaEdtechDbContext _dbContext;
	private readonly ISchoolRepository _schoolRepository;

	public SchoolsController(
		GamaEdtechDbContext dbContext, 
		ISchoolRepository schoolRepository)
	{
		_dbContext = dbContext;
		_schoolRepository = schoolRepository;
	}

	[HttpGet("geo-search")]
	public async Task<IActionResult> SearchByLocation([FromQuery] LocationDto dto)
	{
		var schools = await _schoolRepository.Find(location: new Location(
			latitude: dto.Latitude,
			longitude: dto.Longitude));

		return Ok(schools.Select(x => new SchoolInListDto
		{
			Name = new SchoolNameDto
			{
				InEnglish = x.Name.InEnglish,
				InLocalLanguage = x.Name.InLocalLanguage,
			},
			Type = x.Type == SchoolType.Public ? 0 : 1
		}));
	}

	[HttpPost]
	public async Task<IActionResult> RegisterNewSchool(
		[FromBody] RegisterNewSchoolDto dto)
	{
		var school = new School(
			name: new SchoolName(
				inEnglish: dto.Name.InEnglish, 
				inLocalLanguage: dto.Name.InLocalLanguage),
			type: dto.Type == 0 ? SchoolType.Public : SchoolType.Private,
			address: new Address(
				description: dto.Address.Description, 
				location: new Location(
					latitude: dto.Address.Location.Latitude, 
					longitude: dto.Address.Location.Longitude),
				country: dto.Address.Country,
				state: dto.Address.State,
				city: dto.Address.City,
				zipCode: dto.Address.ZipCode));

		await _schoolRepository.Add(school);
		await _dbContext.SaveChangesAsync();

		return Created();
	}
}

