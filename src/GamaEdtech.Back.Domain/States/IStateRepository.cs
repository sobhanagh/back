

namespace GamaEdtech.Back.Domain.States;

public interface IStateRepository
{
	public Task<State?> GetBy(Guid id);
	public Task<bool> ContainsStateWithNameInCountry(string name, Guid countryId);
	public Task<bool> ContainsStateWithCodeInCountry(string code, Guid countryId);
	public Task Add(State state);
	public Task Remove(State state);
}
