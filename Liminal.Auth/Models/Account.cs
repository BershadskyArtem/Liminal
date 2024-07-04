namespace Liminal.Auth.Models;

public class Account
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = "register";
    public string Email { get; set; }
    public bool IsConfirmed { get; private set; }
    public Guid UserId { get; set; }
    public ICollection<AccountToken> Passwords { get; set; } = new List<AccountToken>();

    protected Account()
    { }

    public void Confirm()
    {
        IsConfirmed = true;
    }

    public void UnConfirm()
    {
        IsConfirmed = false;
    }
    
    public static Account CreateConfirmed(string provider, string email, Guid linkedUserId)
    {
        return Create(provider, email, true, linkedUserId);
    }
    
    public static Account CreateNotConfirmed(string provider, string email, Guid linkedUserId)
    {
        return Create(provider, email, false, linkedUserId);
    }
    
    public static Account Create(string provider, string email, bool isConfirmed, Guid linkedUserId)
    {
        return new Account()
        {
            Id = Guid.NewGuid(),
            Email = email,
            IsConfirmed = isConfirmed,
            UserId = linkedUserId,
            Provider = provider,
        };
    }
    
}