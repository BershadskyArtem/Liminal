// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Jwt;

public class JwtTokenGeneratorOptions(IServiceCollection services): AbstractOptions(services)
{
    public SecurityKey CryptoKey { get; set; } = default!;
    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(5);

    public override void Build()
    { }
}
