using System.Security.Claims;

namespace Liminal.Auth.Models;

public abstract class AbstractUser
{
    public Guid Id { get; set; }
    public virtual required string Email { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual Guid RoleId { get; set; }
    public bool IsConfirmed { get; private set; }
    public virtual Role Role { get; set; } = default!;
    public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    public virtual ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();

    public void Confirm()
    {
        IsConfirmed = true;
    }
    
    public void UnConfirm()
    {
        IsConfirmed = false;
    }
    
    public ClaimsPrincipal ToPrincipal()
    {
        var identity = new ClaimsIdentity();
        
        identity.AddClaim(new Claim("sub", Id.ToString()));
        identity.AddClaim(new Claim("confirmed", IsConfirmed.ToString()));

        return new ClaimsPrincipal(identity);
    }
}