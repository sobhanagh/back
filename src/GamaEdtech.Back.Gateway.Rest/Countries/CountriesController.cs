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
	public async Task<IActionResult> AddCountry([FromBody] AddCountryDto dto)
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

	[HttpPatch("{id:guid}")]
	public async Task<IActionResult> EditCountryInfo(
		[FromRoute] Guid id, [FromBody] EditCountryInfoDto dto)
	{
		var country = await _countryRepository.GetBy(id);

		if (country is null)
			return NotFound();

		if (await _countryRepository.ContainsCountrywithName(dto.Name))
			return BadRequest(Envelope.Error("name is duplicate"));

		if (await _countryRepository.ContainsCountrywithCode(dto.Code))
			return BadRequest(Envelope.Error("code is duplicate"));

		country.EditInfo(dto.Name, dto.Code);

		await _dbCotext.SaveChangesAsync();

		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> RemoveCountry([FromRoute] Guid id)
	{
		var country = await _countryRepository.GetBy(id);

		if (country is null)
			return NotFound();

		await _countryRepository.Remove(country);
		await _dbCotext.SaveChangesAsync();

		return NoContent();
	}
}
