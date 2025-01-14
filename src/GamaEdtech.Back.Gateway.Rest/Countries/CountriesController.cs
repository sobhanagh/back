using Dapper;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Base;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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

	///<summary>
	/// Find Countries
	///</summary>
	/// 
	///<remarks>
	/// Sample request:
	/// 
	///     GET /Countries
	///     
	///		Query params:
	///		{
	///			"page": int - Positive - Default(1),
	///			"pageSize": int - Range Between [1, 50], Default(10), 
	///			"sortBy": "Name" or "Code" - Default("Name"),
	///			"order": "ASC" or "DESC" - Default("ASC"),
	///		}
	///</remarks>
	///
	///<response code="200">Returns list of contries 
	///						(returns empty list if no country is found based on search queries)
	///</response>
	///<response code="400"></response>
	///<response code="500">Server error</response>
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

	///<summary>
	/// Add Country
	///</summary>
	/// 
	///<remarks>
	/// Sample request:
	///
	///     POST /Countries
	///     
	///     Request body:
	///     {
	///		  "name": Required - MaxLenght(50),
	///		  "code": Required - ISO Alpha-2/Alpha-3
	///		}
	///</remarks>
	///
	///<response code="201"></response>
	///<response code="400"></response>
	///<response code="500">Server error</response>
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

	///<summary>
	/// Edit Country info
	///</summary>
	/// 
	///<remarks>
	/// Sample request:
	///
	///     PATCH /Countries/{id:int}
	///     
	///     Request body:
	///     {
	///		  "name": Required - MaxLenght(50),
	///		  "code": Required - ISO Alpha-2/Alpha-3
	///		}
	///</remarks>
	///
	///<response code="204"></response>
	///<response code="400"></response>
	///<response code="404"></response>
	///<response code="500">Server error</response>
	[HttpPut("{id:guid}")]
	public async Task<IActionResult> EditCountryInfo(
		[FromRoute] int id, [FromBody] EditCountryInfoDto dto)
	{
		var country = await _countryRepository.GetBy(new Id(id));

		if (country is null)
			return NotFound();

		if (dto.Name != country.Name && await _countryRepository.ContainsCountrywithName(dto.Name))
			return BadRequest(Envelope.Error("name is duplicate"));

		if (dto.Code != country.Code && await _countryRepository.ContainsCountrywithCode(dto.Code))
			return BadRequest(Envelope.Error("code is duplicate"));

		country.EditInfo(dto.Name, dto.Code);

		await _dbCotext.SaveChangesAsync();

		return NoContent();
	}

	///<summary>
	/// Remove Country
	///</summary>
	/// 
	///<remarks>
	/// Sample request:
	///
	///     DELETE /Countries/{id:int}
	///</remarks>
	///
	///<response code="204"></response>
	///<response code="404"></response>
	///<response code="500">Server error</response>
	[HttpDelete("{id:int}")]
	public async Task<IActionResult> RemoveCountry([FromRoute] int id)
	{
		var country = await _countryRepository.GetBy(new Id(id));

		if (country is null)
			return NotFound();

		await _countryRepository.Remove(country);
		await _dbCotext.SaveChangesAsync();

		return NoContent();
	}
}
