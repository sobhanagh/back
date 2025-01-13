using GamaEdtech.Back.Domain.Base;

namespace GamaEdtech.Back.Domain.Countries;

public class Country : AggregateRoot
{
	public string Name { get; private set; }
	public string Code { get; private set; }

	public Country(string name, string code)
	{
		Name = name;
		Code = code;
	}

	public void EditInfo(string name, string code)
	{
		Name = name;
		Code = code;
	}
}
