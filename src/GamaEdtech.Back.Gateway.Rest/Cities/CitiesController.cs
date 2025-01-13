using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Base;
using GamaEdtech.Back.Domain.Cities;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Domain.States;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.AspNetCore.Mvc;
namespace GamaEdtech.Back.Gateway.Rest.Cities;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
	private readonly GamaEdtechDbContext _dbContext;
	private readonly IStateRepository _stateRepository;
	private readonly ICountryRepository _countryRepository;
	private readonly ICityRepository _cityRepository;

	public CitiesController(
		GamaEdtechDbContext dbContext, 
		IStateRepository stateRepository, 
		ICountryRepository countryRepository, 
		ICityRepository cityRepository)
	{
		_dbContext = dbContext;
		_stateRepository = stateRepository;
		_countryRepository = countryRepository;
		_cityRepository = cityRepository;
	}

	[HttpPost]
	public async Task<IActionResult> AddCity([FromBody] AddCityDto dto)
	{
		if(dto.StateId.HasValue)
		{
			var state = await _stateRepository.GetBy(new Id(dto.StateId.Value));
			if(state is null)
				return NotFound();

			if(await _cityRepository.ContainsCityWithNameInState(dto.Name, state.Id))
				return BadRequest(Envelope.Error("name is duplicate"));

			await _cityRepository.Add(state.CreateCityWith(dto.Name));
		}
		else
		{
			var country = await _countryRepository.GetBy(new Id(dto.CountryId));
			if (country is null)
				return NotFound();

			if (await _cityRepository.ContainsCityWithNameInCountry(dto.Name, country.Id))
				return BadRequest(Envelope.Error("name is duplicate"));

			await _cityRepository.Add(country.CreateCityWith(dto.Name));
		}

		await _dbContext.SaveChangesAsync();

		return Created();
	}
}
