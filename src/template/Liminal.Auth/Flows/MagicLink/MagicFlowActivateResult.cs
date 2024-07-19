// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Liminal.Auth.Results;

namespace Liminal.Auth.Flows.MagicLink;

public class MagicFlowActivateResult : ResultBase
{
    public ClaimsPrincipal Principal { get; set; } = default!;

    public string Email { get; set; } = default!;

    protected MagicFlowActivateResult()
    {
    }

    public static MagicFlowActivateResult Failure() => new MagicFlowActivateResult
    {
        IsSuccess = false,
    };

    public static MagicFlowActivateResult Success(ClaimsPrincipal principal, string email) => new MagicFlowActivateResult
    {
        Principal = principal,
        Email = email,
        IsSuccess = true,
    };
}
