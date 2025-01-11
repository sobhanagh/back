using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GamaEdtech.Back.Gateway.Rest.Countries;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
	private readonly GamaEdtechDbContext _dbCotext;
	private readonly ICountryRepository _countryRepository;

	public CountriesController(
		GamaEdtechDbContext dbCotext, 
		ICountryRepository countryRepository)
	{
		_dbCotext = dbCotext;
		_countryRepository = countryRepository;
	}

	[HttpPost]
	public async Task<IActionResult> AddCountry(AddCountryDto dto)
	{
		if (await _countryRepository.ContainsCountrywithName(dto.Name))
			return BadRequest(Envelope.Error("name is duplicate"));

		if (await _countryRepository.ContainsCountrywithCode(dto.Code))
			return BadRequest(Envelope.Error("code is duplicate"));

		var country = new Country(dto.Name, dto.Code);

		await _countryRepository.Add(country);
		await _dbCotext.SaveChangesAsync();

		return Created();
	}
}
