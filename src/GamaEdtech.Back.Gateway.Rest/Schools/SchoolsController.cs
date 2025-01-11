using CSharpFunctionalExtensions;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Schools;
using GamaEdtech.Back.Gateway.Rest.Utils;
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

	[HttpGet]
	public async Task<IActionResult> Search([FromQuery] string? name)
	{
		var schools = await _schoolRepository.Find(name);

		return Ok(schools.Select(x => new SchoolInListDto
		{
			Id = x.Id,
			Name = new SchoolNameDto
			{
				InEnglish = x.Name.InEnglish,
				InLocalLanguage = x.Name.InLocalLanguage,
			},
			Type = x.Type == SchoolType.Public ? 0 : 1
		}));
	}

	[HttpGet("geo-search")]
	public async Task<IActionResult> SearchByLocation(
		[FromQuery] SearchByLocationDto dto)
	{
		var location = new Location(
				latitude: dto.Latitude,
				longitude: dto.Longitude);

		var schools = await _schoolRepository.FindByLocation(
			location: location,
			radiusInKm: dto.RadiusInKm);

		return Ok(schools.Select(x => new SchoolInListDto
		{
			Id = x.Id,
			Name = new SchoolNameDto
			{
				InEnglish = x.Name.InEnglish,
				InLocalLanguage = x.Name.InLocalLanguage,
			},
			Type = x.Type == SchoolType.Public ? 0 : 1
		}));
	}

	[HttpPost]
	public async Task<IActionResult> RegisterSchool(
		[FromBody] RegisterSchoolDto dto)
	{
		var name = new SchoolName(
				inEnglish: dto.Name.InEnglish,
				inLocalLanguage: dto.Name.InLocalLanguage);

		var location = new Location(
					latitude: dto.Address.Location.Latitude,
					longitude: dto.Address.Location.Longitude);

		var address = new Address(
				description: dto.Address.Description,
				location: location,
				country: dto.Address.Country,
				state: dto.Address.State,
				city: dto.Address.City,
				zipCode: dto.Address.ZipCode);


		var school = new School(
			name: name,
			type: dto.Type == 0 ? SchoolType.Public : SchoolType.Private,
			address: address);

		await _schoolRepository.Add(school);
		await _dbContext.SaveChangesAsync();

		return Created();
	}
}

