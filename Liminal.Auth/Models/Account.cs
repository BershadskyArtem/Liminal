namespace Liminal.Auth.Models;

public class Account
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = "register";
    public string Email { get; set; }
    public bool IsConfirmed { get; set; } = false;
    public Guid UserId { get; set; }
    public ICollection<AccountToken> Passwords { get; set; } = new List<AccountToken>();
}