// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Diagnostics.CodeAnalysis;

namespace Liminal.Auth.Models;
using System.Security.Claims;
using Liminal.Common.Domain.Models;

[SuppressMessage(
    "Usage",
    "CA2227:Collection properties should be read only",
    Justification = "Needed for EF.")]
public class AbstractUser : AuditableEntity
{
    public virtual required string Email { get; set; }
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual bool IsConfirmed { get; private set; }
    public virtual string Role { get; set; } = "default";
    public virtual string? Username { get; set; }
    public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();
    public virtual ICollection<UserToken> Tokens { get; set; } = new List<UserToken>();

    public void Confirm() => this.IsConfirmed = true;

    public void UnConfirm() => this.IsConfirmed = false;

    public ClaimsPrincipal ToPrincipal()
    {
        var identity = new ClaimsIdentity();

        identity.AddClaim(new Claim("sub", this.Id.ToString()));
        identity.AddClaim(new Claim("confirmed", this.IsConfirmed.ToString()));
        identity.AddClaim(new Claim(ClaimTypes.Role, this.Role));

        return new ClaimsPrincipal(identity);
    }
}
