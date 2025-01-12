using Dapper;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Domain.States;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GamaEdtech.Back.Gateway.Rest.States;

[Route("api/[controller]")]
[ApiController]
public class StatesController : ControllerBase
{
	private readonly ConnectionString _connectionString;
	private readonly GamaEdtechDbContext _dbContext;
	private readonly ICountryRepository _countryRepository;
	private readonly IStateRepository _stateRepository;

	public StatesController(
		ConnectionString connectionString,
		GamaEdtechDbContext dbContext,
		ICountryRepository countryRepository,
		IStateRepository stateRepository)
	{
		_connectionString = connectionString;
		_dbContext = dbContext;
		_countryRepository = countryRepository;
		_stateRepository = stateRepository;
	}

	[HttpGet]
	public async Task<IActionResult> FindStates([FromQuery] FindStatesDto dto)
	{
		var query = @"
            SELECT [Id], [Name], [Code], [CountryId]
            FROM [GamaEdtech].[dbo].[State] "
			+ (dto.CountryId.HasValue ?  @"WHERE [CountryId] = @CountryId " : " ") +
            "ORDER BY [" + dto.SortBy + "] " + dto.Order + @"
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

		using (var connection = new SqlConnection(_connectionString.Value))
		{
			var contries = await connection.QueryAsync<StateInListDto>(
				query,
				new
				{
					CountryId = dto.CountryId,
					Offset = (dto.Page - 1) * dto.PageSize,
					PageSize = dto.PageSize,
				});

			return Ok(Envelope.Ok(contries));
		}
	}

	[HttpPost]
	public async Task<IActionResult> AddStateToCountry([FromBody] AddStateDto dto)
	{
		var country = await _countryRepository.GetBy(dto.CountryId);

		if (country is null)
			return NotFound();

		if (await _stateRepository.ContainsStateWithNameInCountry(dto.Name, country.Id))
			return BadRequest(Envelope.Error("name is duplicate"));

		if (await _stateRepository.ContainsStateWithCodeInCountry(dto.Code, country.Id))
			return BadRequest(Envelope.Error("code is duplicate"));

		var state = new State(dto.Name, dto.Code, country.Id);

		await _stateRepository.Add(state);
		await _dbContext.SaveChangesAsync();

		return Created();
	}

	[HttpPut("{id:guid}/EditInfo")]
	public async Task<IActionResult> EditStateInfo(
		[FromRoute] Guid id, [FromBody] EditStateInfoDto dto)
	{
		var state = await _stateRepository.GetBy(id);

		if (state is null)
			return NotFound();

		if (dto.Name != state.Name && await _stateRepository.ContainsStateWithNameInCountry(dto.Name, state.CountryId))
			return BadRequest(Envelope.Error("name is duplicate"));

		if (dto.Code != state.Code && await _stateRepository.ContainsStateWithCodeInCountry(dto.Code, state.CountryId))
			return BadRequest(Envelope.Error("code is duplicate"));

		state.EditInfo(dto.Name, dto.Code);

		await _dbContext.SaveChangesAsync();

		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	public async Task<IActionResult> RemoveState([FromRoute] Guid id)
	{
		var state = await _stateRepository.GetBy(id);

		if (state is null)
			return NotFound();

		await _stateRepository.Remove(state);
		await _dbContext.SaveChangesAsync();

		return NoContent();
	}
}
