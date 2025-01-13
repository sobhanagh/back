

namespace GamaEdtech.Back.Domain.States;

public interface IStateRepository
{
	public Task<State?> GetBy(int id);
	public Task<bool> ContainsStateWithNameInCountry(string name, int countryId);
	public Task<bool> ContainsStateWithCodeInCountry(string code, int countryId);
	public Task Add(State state);
	public Task Remove(State state);
}
