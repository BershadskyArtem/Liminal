// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IAccountLinker<in TUser>
    where TUser : AbstractUser
{
    public Task<bool> LinkAccount(TUser user, Account account, bool save = false);

    public Task<bool> UnlinkAccount(TUser user, Account account, Func<TUser> userFactory, bool save = false);
}
