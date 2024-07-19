// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;
using Liminal.Auth.Options;

namespace Liminal.Auth.Flows.Password;

public static class TokenGeneratorOptionsExtensions
{
    public static TokenGeneratorBuilder AddPasswordFlow<TUser>(
        this TokenGeneratorBuilder builder)
        where TUser : AbstractUser => builder.AddPasswordFlow<TUser>(_ => { });

    public static TokenGeneratorBuilder AddPasswordFlow<TUser>(
        this TokenGeneratorBuilder builder,
        Action<PasswordFlowOptions> configure)
        where TUser : AbstractUser
    {
        var options = new PasswordFlowOptions(builder.Services);

        configure(options);

        return builder.AddFlow<PasswordFlow<TUser>, PasswordFlowOptions>(options);
    }
}
