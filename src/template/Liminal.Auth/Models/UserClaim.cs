// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Liminal.Auth.Models;

[SuppressMessage(
    "ReSharper",
    "EntityFramework.ModelValidation.UnlimitedStringLength",
    Justification = "Solved in another module.")]
[SuppressMessage(
    "ReSharper",
    "VirtualMemberCallInConstructor",
    Justification = "Fine for EF.")]
public class UserClaim
{
    public virtual Guid Id { get; set; }

    public virtual Guid UserId { get; set; }

    public virtual string ClaimType { get; set; } = default!;

    public virtual string ClaimValue { get; set; } = default!;

    public static UserClaim FromClaim(Claim claim)
        => new UserClaim
        {
            ClaimType = claim.Type,
            ClaimValue = claim.Value,
        };

    public Claim ToClaim() => new Claim(this.ClaimType, this.ClaimValue);
}
