using CSharpFunctionalExtensions;
using GamaEdtech.Back.Domain.Countries;

namespace GamaEdtech.Back.Domain.States;

public class State : Entity<int>
{
	public string Name { get; private set; }
	public string Code { get; private set; }
	public int CountryId { get; private set; }

	public State(string name, string code, int countryId)
	{
		Name = name;
		Code = code;
		CountryId = countryId;
	}

	public void EditInfo(string name, string code)
	{
		Name = name; 
		Code = code;
	}

	public void MoveTo(Country country)
	{
		CountryId = country.Id;
	}
}
