// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace Liminal.Auth.Models;

[SuppressMessage(
    "Usage",
    "CA2227:Collection properties should be read only",
    Justification = "Needed for EF.")]
[SuppressMessage(
    "ReSharper",
    "EntityFramework.ModelValidation.UnlimitedStringLength",
    Justification = "Solved in another module.")]
public class Account
{
    public Guid Id { get; set; }

    public string Provider { get; set; } = "register";

    public string Email { get; set; } = default!;

    public bool IsConfirmed { get; private set; }

    public Guid UserId { get; set; }

    // ReSharper disable once CollectionNeverUpdated.Global
    public ICollection<AccountToken> Passwords { get; set; } = new List<AccountToken>();

    protected Account()
    { }

    public static Account CreateConfirmed(string provider, string email, Guid linkedUserId)
        => Create(provider, email, true, linkedUserId);

    public static Account CreateNotConfirmed(string provider, string email, Guid linkedUserId)
        => Create(provider, email, false, linkedUserId);

    public static Account Create(string provider, string email, bool isConfirmed, Guid linkedUserId) => new Account
    {
        Id = Guid.NewGuid(),
        Email = email,
        IsConfirmed = isConfirmed,
        UserId = linkedUserId,
        Provider = provider,
    };

    public void Confirm() => this.IsConfirmed = true;

    public void UnConfirm() => this.IsConfirmed = false;
}
