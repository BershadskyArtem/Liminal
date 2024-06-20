namespace Liminal.Auth.Abstractions;

public interface IPasswordStore
{
    public Task<string?> GetHashedPasswordByEmailAsync(string email);
    public Task SetPasswordAsync(string email, Guid accountId, string hashedPassword);
    public Task<bool> SetOrAddTokenAsync(Guid userId, Guid accountId, string tokenName, string tokenValue);
    Task<string?> GetTokenByEmailAsync(string email, string providerName, string accessToken);
}