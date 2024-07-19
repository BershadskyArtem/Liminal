// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IAuthContext<TUser>
    where TUser : AbstractUser
{
    public Guid? UserId { get; set; }

    public bool IsConfirmed { get; set; }

    public Task<string?> Role();

    public Task<TUser?> Current();
}
