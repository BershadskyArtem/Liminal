namespace Liminal.Auth.Flows.OAuth.Providers;

public interface IOAuthProvidersProvider
{
    public IOAuthProvider GetProvider(string key);
}