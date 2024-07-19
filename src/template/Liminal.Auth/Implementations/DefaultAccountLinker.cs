// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.Implementations;

public class DefaultAccountLinker<TUser>(
    IUserStore<TUser> userStore,
    IAccountStore accountStore)
    : IAccountLinker<TUser>
    where TUser : AbstractUser
{
    public Task<bool> LinkAccount(TUser user, Account account, bool save = false) => throw new NotImplementedException();

    public async Task<bool> UnlinkAccount(TUser user, Account account, Func<TUser> userFactory, bool save = false)
    {
        if (!user.IsConfirmed || !account.IsConfirmed)
        {
            return false;
        }

        if (account.UserId != user.Id)
        {
            return true;
        }

        var newUser = userFactory();
        newUser.Id = Guid.NewGuid();
        newUser.Confirm();
        newUser.Email = account.Email;

        var success = await userStore.AddAsync(newUser, true);

        if (!success)
        {
            return success;
        }

        account.UserId = newUser.Id;

        return await accountStore.UpdateAsync(account, true);
    }
}
