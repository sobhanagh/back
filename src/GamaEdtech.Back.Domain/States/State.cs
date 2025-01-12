using CSharpFunctionalExtensions;

namespace GamaEdtech.Back.Domain.States;

public class State : Entity<Guid>
{
	public string Name { get; set; }
	public string Code { get; set; }
	public Guid CountryId { get; set; }

	public State(string name, string code, Guid countryId)
	{
		Name = name;
		Code = code;
		CountryId = countryId;
	}
}
