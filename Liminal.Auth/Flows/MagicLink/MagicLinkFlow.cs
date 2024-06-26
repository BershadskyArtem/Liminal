using Liminal.Auth.Abstractions;
using Liminal.Auth.Common;
using Liminal.Auth.Models;
using Liminal.Mail;

namespace Liminal.Auth.Flows.MagicLink;

public class MagicLinkFlow<TUser>(
    MagicLinkOptions options,
    AbstractMailer mailer,
    IPasswordStore passwordStore,
    IAccountStore accountStore,
    IUserStore<TUser> userStore) : IAuthFlow 
    where TUser : AbstractUser
{
    public string Name { get; } = MagicLinkDefaults.Scheme;

    public async Task<bool> SendLink(string email, Func<TUser> factory)
    {
        var existingUser = await userStore.GetByEmailAsync(email);

        if (existingUser is null)
        {
            existingUser = factory();

            await userStore.AddAsync(existingUser, true);
        }
        
        var existingAccount = await accountStore.GetByProviderAsync(existingUser.Id, Name);
        
        if (existingAccount is null)
        {
            existingAccount = new Account()
            {
                Email = email,
                Id = Guid.NewGuid(),
                Provider = Name,
                UserId = existingUser.Id
            };

            await accountStore.AddAsync(existingAccount, true);
        }
        
        var token = CryptoUtils.GenerateRandomString(64);
        var password = AccountToken.Create(existingAccount.Id, Name, Name, token);
        await passwordStore.AddAsync(password, true);
        
        var sent = await mailer.SendEmailAsync(email, $"""
                                     Hello, your confirmation link is {options.ActivateUrl}/{token}
                                     """);
        return sent;
    }

    public async Task<MagicFlowActivateResult> ActivateAsync(string token)
    {
        var password= await passwordStore.GetByValueAsync(Name, token);
        if (password is null)
        {
            return MagicFlowActivateResult.Failure();
        }

        var account = await accountStore.GetByIdAsync(password.AccountId);
        if (account is null)
        {
            return MagicFlowActivateResult.Failure();
        }

        var user = await userStore.GetByIdAsync(account.UserId);
        if (user is null)
        {
            return MagicFlowActivateResult.Failure();
        }
        
        if (!account.IsConfirmed)
        {
            account.IsConfirmed = true;
            await accountStore.UpdateAsync(account ,true);
        }

        if (!user.IsConfirmed)
        {
            user.IsConfirmed = true;
            await userStore.UpdateAsync(user ,true);
        }
        
        await passwordStore.RemoveAsync(password, true);
        
        var principal = user.ToPrincipal();
        return MagicFlowActivateResult.Success(principal, account.Email);
    }
}