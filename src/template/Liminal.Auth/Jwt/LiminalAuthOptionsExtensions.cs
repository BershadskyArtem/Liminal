// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Models;
using Liminal.Auth.Options;

namespace Liminal.Auth.Jwt;

public static class LiminalAuthOptionsExtensions
{
    public static TokenGeneratorBuilder AddJwtTokenGenerator<TUser>(
        this LiminalAuthBuilder builder,
        Action<JwtTokenGeneratorOptions> configure)
        where TUser : AbstractUser
    {
        var options = new JwtTokenGeneratorOptions(builder.Services);

        configure(options);

        return builder.AddTokenGenerator<JwtTokenGenerator<TUser>, JwtTokenGeneratorOptions>(JwtDefaults.Scheme, options);
    }
}
