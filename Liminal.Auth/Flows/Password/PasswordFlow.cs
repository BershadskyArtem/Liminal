using Liminal.Auth.Abstractions;
using Liminal.Auth.Common;
using Liminal.Auth.Models;
using Liminal.Auth.Results;
using Liminal.Mail;

namespace Liminal.Auth.Flows.Password;

public class PasswordFlow<TUser>(
    PasswordFlowOptions options,
    IUserStore<TUser> userStore,
    IPasswordStore passwordStore,
    IAccountStore accountStore,
    AbstractMailer mailer)
    : IAuthFlow
    where TUser : AbstractUser
{
    public string Name { get; } = PasswordDefaults.Scheme;

    public async Task<bool> Register(string email, string password, Func<TUser> factory)
    {
        var user = await userStore.GetByEmailAsync(email);

        if (user is not null)
        {
            return false;
        }

        user = factory();
        user.Id = Guid.NewGuid();
        user.Email = email;

        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        bool success = false;
        success = await userStore.AddAsync(user);

        if (!success)
        {
            return false;
        }
        
        var account = new Account()
        {
            Id = Guid.NewGuid(),
            Email = user.Email,
            Passwords = new List<AccountToken>(),
            Provider = Name,
            UserId = user.Id
        };

        success = await accountStore.AddAsync(account, true);

        if (!success)
        {
            return false;
        }

        success = await AddPasswordAsync(account, hashedPassword);
        
        if (!success)
        {
            return false;
        }

        return await SendConfirmationEmail(account);
    }

    public async Task<bool> SendConfirmationEmail(string email)
    {
        var account = await accountStore.GetByProviderAsync(email, Name);
        if (account is null)
        {
            return false;
        }

        return await SendConfirmationEmail(account);
    }
    
    
    public async Task<bool> SendConfirmationEmail(Account account)
    {
        if (account.Provider != Name)
        {
            return false;
        }
        
        var token = CryptoUtils.GenerateRandomString(64);
        var password = AccountToken.Create(account.Id, Name, Name, token);
        var success = await passwordStore.AddAsync(password, true);

        if (!success)
        {
            return false;
        }
        
        var sent = await mailer.SendEmailAsync(account.Email, $"""
                                                       Hello, your confirmation link is {options.ActivateUrl}/{token}
                                                       """);
        return sent;
    }

    public async Task<CallbackResult<TUser>> ActivateAccount(string token, string redirectAfter)
    {
        var activationToken = await passwordStore.GetByValueAsync(Name, token);
        if (activationToken is null)
        {
            return CallbackResult<TUser>.Failure("Activation token does not exist.");
        }

        var account = await accountStore.GetByIdAsync(activationToken.AccountId);
        if (account is null)
        {
            return CallbackResult<TUser>.Failure("Account does not exist.");
        }
        
        var user = await userStore.GetByIdAsync(account.UserId);
        if (user is null)
        {
            return CallbackResult<TUser>.Failure("User does not exist.");
        }

        user.IsConfirmed = true;
        account.IsConfirmed = true;

        bool success = false;
        success = await userStore.UpdateAsync(user, true); 
        
        if (!success)
        {
            return CallbackResult<TUser>.Failure("Cannot update user store.");
        }
        
        success = await accountStore.UpdateAsync(account, true);
        
        if (!success)
        {
            return CallbackResult<TUser>.Failure("Cannot update account store.");
        }
        
        return CallbackResult<TUser>.Success(user.Email, user.ToPrincipal(), redirectAfter);
    }

    public async Task<CallbackResult<TUser>> Login(string email, string password, string redirectAfter)
    {
        if (await CanSignIn(email, password))
        {
            var user = await userStore.GetByEmailAsync(email);

            if (user is null)
            {
                return CallbackResult<TUser>.Failure("Cannot login because user does not exist.");
            }

            var principal = user.ToPrincipal();

            return CallbackResult<TUser>.Success(user.Email, principal, redirectAfter);
        }

        return CallbackResult<TUser>.Failure("Invalid credentials");
    }
    
    private async Task<bool> AddPasswordAsync(Account account, string hashedPassword)
    {
        var password = AccountToken.Create(
            account.Id,
            Name,
            Name,
            hashedPassword);

        return await passwordStore.AddAsync(password, true);
    }

    private async Task<string?> GetPasswordAsync(string email)
    {
        var account = await accountStore.GetByProviderAsync(email, Name);

        if (account is null)
        {
            return null;
        }

        var password = await passwordStore.GetByAccountIdAsync(account.Id, Name);

        return password?.TokenValue;
    } 

    public async Task<bool> CanSignIn(string email, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var hashedPassword = await GetPasswordAsync(email);

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
    
}