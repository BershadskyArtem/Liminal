// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    IUserFactory<TUser> userFactory,
    IUserStore<TUser> userStore): IAuthFlow
    where TUser : AbstractUser
{
    public string Name { get; } = MagicLinkDefaults.Scheme;

    public async Task<bool> SendLink(string email)
    {
        var existingAccount = await accountStore.GetByProviderAsync(email, this.Name);
        TUser? existingUser = null;

        // If account does not exist then we need to get the user with the given email.
        // If the user is verified then we create account and link.
        // If the user is not verified we do not link and instead generate new user.
        if (existingAccount is null)
        {
            existingUser = await userStore.GetByEmailAsync(email);

            // If the user does not exist or is not confirmed then we create one.
            // Do not allow not confirmed account linking.
            if (existingUser is null)
            {
                existingUser = userFactory.CreateConfirmed(email);
                existingUser.Role = options.DefaultRole;

                await userStore.AddAsync(existingUser, true);
            }

            if (!existingUser.IsConfirmed)
            {
                existingUser.Confirm();
                await userStore.UpdateAsync(existingUser, true);
            }

            existingAccount = Account.CreateConfirmed(this.Name, email, existingUser.Id);

            await accountStore.AddAsync(existingAccount, true);
        }

        // If the account exists that means that we have already done some type of flow before.
        // Therefore, user MUST EXIST and BE CONFIRMED.
        existingUser ??= await userStore.GetByEmailAsync(email);

        if (existingUser is null || !existingUser.IsConfirmed)
        {
            return false;
        }

        var token = CryptoUtils.GenerateRandomString(64);
        var password = AccountToken.Create(existingAccount.Id, this.Name, this.Name, token, null);
        await passwordStore.AddAsync(password, true);

#pragma warning disable SA1118
        var sent = await mailer.SendEmailAsync(email, $"""
                                                       Hello, your confirmation link is {options.ActivateUrl}?code={token}
                                                       """);
#pragma warning restore SA1118
        return sent;
    }

    public async Task<MagicFlowActivateResult> ActivateAsync(string token)
    {
        var password = await passwordStore.GetByValueAsync(this.Name, token);
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
}
