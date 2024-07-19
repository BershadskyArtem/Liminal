// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace Liminal.Auth.Models;
using System.Security.Claims;

[SuppressMessage(
    "ReSharper",
    "EntityFramework.ModelValidation.UnlimitedStringLength",
    Justification = "Solved in another module.")]
public class AccountClaim
{
    public virtual Guid Id { get; set; }
    public virtual Guid AccountId { get; set; }
    public virtual string ClaimType { get; set; } = default!;
    public virtual string ClaimValue { get; set; } = default!;

    public static AccountClaim FromClaim(Claim claim, Guid userId) => new AccountClaim
    {
        ClaimType = claim.Type,
        ClaimValue = claim.Value,
        AccountId = userId,
        Id = Guid.NewGuid(),
    };

    public Claim ToClaim() => new Claim(this.ClaimType, this.ClaimValue);
}
