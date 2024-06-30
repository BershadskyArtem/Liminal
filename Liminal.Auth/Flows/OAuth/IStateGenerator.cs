namespace Liminal.Auth.Flows.OAuth;

public interface IStateGenerator
{
    public string GenerateState(string provider, string redirectAfter, Guid? linkingTargetId = null);
    public Task<State> ParseState(string state);
}