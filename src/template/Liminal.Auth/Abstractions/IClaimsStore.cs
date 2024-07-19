// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Liminal.Auth.Abstractions;

public interface IClaimsStore
{
    public Task<bool> AddRangeAsync(IReadOnlyCollection<Claim> claims, Guid userId, Guid accountId, string providerName, bool save = false);

    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
