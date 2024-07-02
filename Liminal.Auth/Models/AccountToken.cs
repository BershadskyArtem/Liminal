namespace Liminal.Auth.Models;

/// <summary>
/// Password record that is used to store user tokens in db. Tokens such as (Refresh, Access)
/// </summary>
public class AccountToken
{
    public virtual Guid Id { get; set; }
    public virtual Guid AccountId { get; set; }
    public virtual string Provider { get; set; } = default!;
    public virtual string TokenName { get; set; } = default!;
    public virtual string TokenValue { get; set; } = default!;
    public virtual Account Account { get; set; } = default!;
    public virtual DateTimeOffset? ValidUntil { get; set; }

    protected AccountToken()
    { }

    protected AccountToken(Guid id, Guid accountId, string provider, string tokenName, string tokenValue, DateTimeOffset? validUntil)
    {
        Id = id;
        AccountId = accountId;
        Provider = provider;
        TokenName = tokenName;
        TokenValue = tokenValue;
        ValidUntil = validUntil;
    }
    
    public static AccountToken Create(Guid accountId, string provider, string tokenName, string tokenValue, DateTimeOffset? validUntil)
    {
        var result = new AccountToken(Guid.NewGuid(), accountId, provider, tokenName, tokenValue, validUntil);
        return result;
    }
    
}