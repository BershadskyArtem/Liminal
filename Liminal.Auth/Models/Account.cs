namespace Liminal.Auth.Models;

public class Account
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = "register";
    public Guid UserId { get; set; }
    public ICollection<Password> Passwords { get; set; } = new List<Password>();
}