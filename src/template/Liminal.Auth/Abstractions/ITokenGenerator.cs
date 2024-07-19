// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Liminal.Auth.Results;

namespace Liminal.Auth.Abstractions;

public interface ITokenGenerator
{
    /// <summary>
    /// Generates an access token from principal.
    /// </summary>
    /// <param name="principal">The user's <see cref="ClaimsPrincipal"/> to generate access token from.</param>
    /// <param name="save">Flag that specifiec whether or not should operation to presist changes immediatly.</param>
    /// <returns>Task with the token result in it.</returns>
    public Task<GenerateTokenResult> GenerateToken(ClaimsPrincipal principal, bool save = true);

    /// <summary>
    /// Refreshes access token using refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<GenerateTokenResult> RefreshToken(string refreshToken);
}
