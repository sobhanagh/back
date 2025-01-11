namespace GamaEdtech.Back.Domain.Countries;

public interface ICountryRepository
{
	public Task Add(Country country);
	public Task<bool> ContainsCountrywithName(string name);
	public Task<bool> ContainsCountrywithCode(string code);
}
