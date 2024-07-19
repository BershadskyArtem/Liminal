// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

/// <summary>
/// Contract for getting and setting <see cref="AccountToken"/> for a given <see cref="Account"/>.
/// </summary>
public interface IPasswordStore
{
    public Task<AccountToken?> GetByAccountIdAsync(Guid accountId, string tokenName);

    public Task<bool> AddAsync(AccountToken accountToken, bool save = false);

    public Task<bool> SetByAccountIdAsync(Guid id, string tokenName, string tokenValue, bool save = false);

    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<bool> UpdateAsync(AccountToken accountToken, bool save = false);

    public Task<AccountToken?> GetByValueAsync(string providerName, string tokenValue);

    public Task<bool> RemoveAsync(AccountToken password, bool save = false);
}
