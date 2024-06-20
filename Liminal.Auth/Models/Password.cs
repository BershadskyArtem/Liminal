namespace Liminal.Auth.Models;

/// <summary>
/// Password record that is used to store user tokens in db. Tokens such as (Refresh, Access)
/// </summary>
public class Password
{
    public virtual Guid Id { get; set; }
    public virtual Guid AccountId { get; set; }
    public virtual string Email { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string TokenName { get; set; } = "password";
    public virtual required string TokenValue { get; set; }
}