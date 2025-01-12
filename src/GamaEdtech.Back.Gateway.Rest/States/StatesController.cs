using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Domain.States;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GamaEdtech.Back.Gateway.Rest.States;

[Route("api/[controller]")]
[ApiController]
public class StatesController : ControllerBase
{
	private readonly GamaEdtechDbContext _dbContext;
	private readonly ICountryRepository _countryRepository;
	private readonly IStateRepository _stateRepository;

	public StatesController(
		GamaEdtechDbContext dbContext,
		ICountryRepository countryRepository,
		IStateRepository stateRepository)
	{
		_dbContext = dbContext;
		_countryRepository = countryRepository;
		_stateRepository = stateRepository;
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
