using Dapper;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Globalization;

namespace GamaEdtech.Back.Gateway.Rest.Countries;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
	private readonly ConnectionString _connectionString;
	private readonly GamaEdtechDbContext _dbCotext;
	private readonly ICountryRepository _countryRepository;

	public CountriesController(
		ConnectionString connectionString,
		GamaEdtechDbContext dbCotext, 
		ICountryRepository countryRepository)
	{
		_connectionString = connectionString;
		_dbCotext = dbCotext;
		_countryRepository = countryRepository;
	}

	[HttpGet]
	public async Task<IActionResult> FindContries([FromQuery] FindCountriesDto dto)
	{
		var query = @"
            SELECT [Id], [Name], [Code]
            FROM [GamaEdtech].[dbo].[Country]
            ORDER BY [" + dto.SortBy + "] " + dto.Order + @"
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

		using (var connection = new SqlConnection(_connectionString.Value))
		{
			var contries = await connection.QueryAsync<ContryInListDto>(
				query,
				new 
				{
					Offset = (dto.Page - 1) * dto.PageSize, 
					PageSize = dto.PageSize,
				});

			return Ok(Envelope.Ok(contries));
		}
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
