namespace Liminal.Auth.Flows.OAuth;

public class UserInfo
{
    public string Email { get; private set; }
    public string UserName { get; private set; }
    public bool IsVerified { get; private set; }

    protected UserInfo()
    { }
    
    public static UserInfo Create(string email, string userName, bool isVerified)
    {
        return new UserInfo
        {
            Email = email,
            UserName = userName,
            IsVerified = isVerified
        };
    }
    
}