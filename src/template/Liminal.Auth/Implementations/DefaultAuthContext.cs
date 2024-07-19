// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;
using Microsoft.AspNetCore.Http;

namespace Liminal.Auth.Implementations;

public class DefaultAuthContext<TUser> : IAuthContext<TUser>
    where TUser : AbstractUser
{
    public DefaultAuthContext(IHttpContextAccessor accessor, IUserStore<TUser> userStore)
    {
        this._userStore = userStore;
        this.UserId = null;
        this.IsConfirmed = false;

        var context = accessor.HttpContext;

        if (context is null)
        {
            return;
        }

        var idClaim = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (!string.IsNullOrWhiteSpace(idClaim) && Guid.TryParse(idClaim, out var id))
        {
            this.UserId = id;
        }

        var confirmedClaim = context.User.Claims.FirstOrDefault(c => c.Type == "confirmed")?.Value;
        if (!string.IsNullOrWhiteSpace(confirmedClaim) && bool.TryParse(confirmedClaim, out var confirmed))
        {
            this.IsConfirmed = confirmed;
        }
    }

    public Guid? UserId { get; set; }

    public bool IsConfirmed { get; set; }

    public async Task<string?> Role() => (await this.Current())?.Role;

    public async Task<TUser?> Current()
    {
        if (this.UserId is null)
        {
            return null;
        }

        if (this._user is not null)
        {
            return this._user;
        }

        this._user = await this._userStore.GetByIdAsync(this.UserId.Value);

        if (this._user is null)
        {
            this.UserId = null;
        }

        return this._user;
    }

    private readonly IUserStore<TUser> _userStore;
    private TUser? _user;
}
