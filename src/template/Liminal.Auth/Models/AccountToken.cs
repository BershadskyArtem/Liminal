// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace Liminal.Auth.Models;

/// <summary>
/// Password record that is used to store user tokens in db. Tokens such as (Refresh, Access).
/// </summary>
[SuppressMessage(
    "ReSharper",
    "EntityFramework.ModelValidation.UnlimitedStringLength",
    Justification = "Solved in another module.")]
[SuppressMessage(
    "ReSharper",
    "VirtualMemberCallInConstructor",
    Justification = "Fine for EF.")]

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AccountToken
{
    public virtual Guid Id { get; set; }

    public virtual Guid AccountId { get; set; }

    public virtual string Provider { get; set; } = default!;

    public virtual string TokenName { get; set; } = default!;

    public virtual string TokenValue { get; set; } = default!;

    public virtual Account Account { get; set; } = default!;

    public virtual DateTimeOffset? ValidUntil { get; set; }

#pragma warning disable S4136
    protected AccountToken()
#pragma warning restore S4136
    { }

    public static AccountToken Create(
        Guid accountId,
        string provider,
        string tokenName,
        string tokenValue,
        DateTimeOffset? validUntil)
    {
        var result = new AccountToken(Guid.NewGuid(), accountId, provider, tokenName, tokenValue, validUntil);
        return result;
    }

    protected AccountToken(
        Guid id,
        Guid accountId,
        string provider,
        string tokenName,
        string tokenValue,
        DateTimeOffset? validUntil)
    {
        Id = id;
        AccountId = accountId;
        Provider = provider;
        TokenName = tokenName;
        TokenValue = tokenValue;
        ValidUntil = validUntil;
    }
}
