// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth;

public class State
{
    public string Provider { get; init; } = default!;
    public string RedirectAfter { get; init; } = default!;
    public string FlowState { get; init; } = default!;
    public string SiteUrl { get; init; } = default!;
    public string RedirectUrl { get; init; } = default!;
    public Guid? TargetUserId { get; set; }
}
