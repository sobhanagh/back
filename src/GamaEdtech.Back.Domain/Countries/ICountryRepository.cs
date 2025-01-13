namespace GamaEdtech.Back.Domain.Countries;

public interface ICountryRepository
{
	public Task<Country?> GetBy(int id);
	public Task<bool> ContainsCountrywithName(string name);
	public Task<bool> ContainsCountrywithCode(string code);
	public Task Add(Country country);
	public Task Remove(Country country);
}
