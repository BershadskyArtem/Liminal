namespace Liminal.Auth.Models;

public abstract class AbstractUser
{
    public Guid Id { get; set; }
    public virtual required string Email { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
    public virtual ICollection<UserClaim> Claims { get; set; }
}