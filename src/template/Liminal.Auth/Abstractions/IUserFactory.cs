// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IUserFactory<out TUser>
    where TUser : AbstractUser
{
    public TUser CreateConfirmed(string email, string? userName = null);

    public TUser CreateUnConfirmed(string email, string? userName = null);
}
