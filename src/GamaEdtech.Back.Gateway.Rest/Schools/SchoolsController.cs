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

	[HttpPost]
	public async Task<IActionResult> RegisterNewSchool(
		[FromBody] RegisterNewSchoolReq req)
	{
		var school = new School(
			name: new SchoolName(
				inEnglish: req.Name.InEnglish, 
				inLocalLanguage: req.Name.InLocalLanguage),
			type: req.Type == 0 ? SchoolType.Public : SchoolType.Private,
			address: new Address(
				description: req.Address.Description, 
				location: new Location(
					latitude: req.Address.Location.Latitude, 
					longitude: req.Address.Location.Longitude),
				country: req.Address.Country,
				state: req.Address.State,
				city: req.Address.City,
				zipCode: req.Address.ZipCode));

		await _schoolRepository.Add(school);
		await _dbContext.SaveChangesAsync();

		return Created();
	}

	
}

