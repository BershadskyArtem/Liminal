using Liminal.Auth.Abstractions;
using Liminal.Auth.Common;
using Liminal.Auth.Models;
using Liminal.Mail;

namespace Liminal.Auth.Flows.MagicLink;

public class MagicLinkFlow<TUser>(
    MagicLinkOptions options,
    AbstractMailer mailer,
    IPasswordStore passwordStore,
    IAccountLinker<TUser> accountLinker,
    IAccountStore accountStore,
    IUserStore<TUser> userStore) : IAuthFlow 
    where TUser : AbstractUser
{
    public string Name { get; } = MagicLinkDefaults.Scheme;

    public async Task<bool> SendLink(string email, Func<TUser> factory)
    {
        var existingAccount = await accountStore.GetByProviderAsync(email, Name);
        TUser? existingUser = null;
        
        // If account does not exist then we need to get the user with the given email.
        // If the user is verified then we create account and link. 
        // If the user is not verified we do not link and instead generate new user.
        if (existingAccount is null)
        {
            existingUser = await userStore.GetByEmailAsync(email);

            // If the user does not exist or is not confirmed then we create one.
            // Do not allow not confirmed account linking.
            if (existingUser is null || !existingUser.IsConfirmed)
            {
                existingUser = CreateUser(email, factory, true);

                await userStore.AddAsync(existingUser, true);
            }

            existingAccount = Account.CreateConfirmed(Name, email, existingUser.Id);

            await accountStore.AddAsync(existingAccount, true);
        }
        
        // If the account exists that means that we have already done some type of flow before.
        // Check if the user is confirmed. 
        // If confirmed then link automatically.
        // If not confirmed create new user.
        existingUser ??= await userStore.GetByEmailAsync(email);
        
        if (existingUser is null || !existingUser.IsConfirmed)
        {
            existingUser = CreateUser(email, factory, true);
            await userStore.AddAsync(existingUser, true);
            existingAccount.UserId = existingUser.Id;
            await accountStore.UpdateAsync(existingAccount, true);
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
            // if account is not confirmed it means that something went horribly wrong at the sending stage
            // TODO: Log this case.
            return MagicFlowActivateResult.Failure();
        }

        if (!user.IsConfirmed)
        {
            // if user is not confirmed it means that something went horribly wrong at the sending stage
            // TODO: Log this case.
            return MagicFlowActivateResult.Failure();
        }
        
        await passwordStore.RemoveAsync(password, true);
        
        var principal = user.ToPrincipal();
        return MagicFlowActivateResult.Success(principal, account.Email);
    }
    
    private static TUser CreateUser(string email, Func<TUser> factory, bool confirmed)
    {
        TUser existingUser;
        existingUser = factory();
        existingUser.Id = Guid.NewGuid();
        existingUser.Email = email;
        if (confirmed)
        {
            existingUser.Confirm();
        }
        else
        {
            existingUser.UnConfirm();
        }
        
        return existingUser;
    }

}