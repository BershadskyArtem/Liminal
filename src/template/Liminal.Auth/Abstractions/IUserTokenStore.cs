// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IUserTokenStore
{
    public Task<IEnumerable<UserToken>> GetByUserId(Guid userId);

    public Task<UserToken?> GetByAccessToken(string accessToken);

    public Task<UserToken?> GetByRefreshToken(string refreshToken);

    public Task<bool> AddAsync(UserToken token, bool save = false);

    public Task<bool> UpdateToken(UserToken token, bool save = false);

    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<bool> RemoveAsync(UserToken tokenSet, bool save = false);
}
