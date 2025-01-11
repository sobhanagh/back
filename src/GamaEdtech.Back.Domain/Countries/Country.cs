using CSharpFunctionalExtensions;

namespace GamaEdtech.Back.Domain.Countries;

public class Country : Entity<Guid>
{
	public string Name { get; private set; }
	public string Code { get; private set; }

	public Country(string name, string code)
	{
		Name = name;
		Code = code;
	}
}
