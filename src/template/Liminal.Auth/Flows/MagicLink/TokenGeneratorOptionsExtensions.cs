// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;
using Liminal.Auth.Options;

namespace Liminal.Auth.Flows.MagicLink;

public static class TokenGeneratorOptionsExtensions
{
    public static TokenGeneratorBuilder AddMagickLink<TUser>(this TokenGeneratorBuilder builder, Action<MagicLinkOptions> cfg)
        where TUser : AbstractUser
    {
        var options = new MagicLinkOptions(builder.Services);

        cfg(options);

        return builder.AddFlow<MagicLinkFlow<TUser>, MagicLinkOptions>(options);
    }
}
