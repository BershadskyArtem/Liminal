// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        var existingAccount = await accountStore.GetByProviderAsync(email, this.Name);

        if (existingAccount is not null)
        {
            // Account already exists.
            // Stupid hackers...
            return false;
        }

        // If the account does not exist then we get the user
        var existingUser = await userStore.GetByEmailAsync(email);

        if (existingUser is null)
        {
            existingUser = factory();
            existingUser.Id = Guid.NewGuid();
            existingUser.Email = email;
            existingUser.UnConfirm();

            await userStore.AddAsync(existingUser, true);
        }
        else
        {
            // Strange case but can happen if hackers really want this.
            return false;
        }

        existingAccount = Account.CreateNotConfirmed(this.Name, email, existingUser.Id);

        await accountStore.AddAsync(existingAccount, true);

        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        var success = await this.AddPasswordAsync(existingAccount, hashedPassword);

        if (!success)
        {
            return false;
        }

        return await SendConfirmationEmail(existingAccount);
    }

    public async Task<CallbackResult<TUser>> Login(string email, string password, string redirectAfter)
    {
        if (await this.CanSignIn(email, password))
        {
            var account = await accountStore.GetByProviderAsync(email, this.Name);

            if (account is null)
            {
                return CallbackResult<TUser>.Failure("Account does not exist.");
            }

            var user = await userStore.GetByIdAsync(account.UserId);

            if (user is null)
            {
                return CallbackResult<TUser>.Failure("Cannot login because user does not exist.");
            }

            var principal = user.ToPrincipal();

            return CallbackResult<TUser>.Success(user.Email, principal, redirectAfter);
        }

        return CallbackResult<TUser>.Failure("Invalid credentials");
    }

    public async Task<bool> SendConfirmationEmail(string email)
    {
        var account = await accountStore.GetByProviderAsync(email, this.Name);
        if (account is null)
        {
            return false;
        }

        return await this.SendConfirmationEmail(account);
    }

    public async Task<bool> SendConfirmationEmail(Account account)
    {
        if (account.Provider != this.Name)
        {
            return false;
        }

        var token = CryptoUtils.GenerateRandomString(64);
        var password = AccountToken.Create(account.Id, this.Name, this.Name, token, null);
        var success = await passwordStore.AddAsync(password, true);

        if (!success)
        {
            return false;
        }

#pragma warning disable SA1118
        var sent = await mailer.SendEmailAsync(account.Email, $"""
                                                               Hello, your confirmation link is {options.ActivateUrl}/{token}
                                                               """);
#pragma warning restore SA1118
        return sent;
    }

    public async Task<CallbackResult<TUser>> ActivateAccount(string token, string redirectAfter)
    {
        var activationToken = await passwordStore.GetByValueAsync(this.Name, token);
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

        user.Confirm();
        account.Confirm();

        var success = await userStore.UpdateAsync(user, true);

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

    public async Task<bool> CanSignIn(string email, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var hashedPassword = await this.GetPasswordAsync(email);

        if (string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }

    private async Task<bool> AddPasswordAsync(Account account, string hashedPassword)
    {
        var password = AccountToken.Create(
            account.Id,
            this.Name,
            this.Name,
            hashedPassword,
            null);

        return await passwordStore.AddAsync(password, true);
    }

    private async Task<string?> GetPasswordAsync(string email)
    {
        var account = await accountStore.GetByProviderAsync(email, this.Name);

        if (account is null)
        {
            return null;
        }

        var password = await passwordStore.GetByAccountIdAsync(account.Id, this.Name);

        return password?.TokenValue;
    }
}
