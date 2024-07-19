// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Flows.OAuth;

public class StateGenerator(OAuthFlowBuilder builder): IStateGenerator
{
    public string GenerateState(string provider, string redirectAfter, Guid? linkingTargetId = null)
    {
        var secretKey = builder.StateCryptoKey;

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(3);

        var claims = new List<Claim>
        {
            new ("provider", provider),
            new ("redirect_after", redirectAfter),
            new ("flow_state", Guid.NewGuid().ToString()),
        };

        if (linkingTargetId is not null)
        {
            claims.Add(new Claim("target_id", linkingTargetId.ToString() !));
        }

        var tokenOptions = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: expires,
            claims: claims);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return token;
    }

    [SuppressMessage(
        "Security",
        "CA5404:Do not disable token validation checks",
        Justification = "There is no aud and iss in state JWT.")]
    public async Task<State> ParseState(string state)
    {
        var securityKey = builder.StateCryptoKey;
        var jwtHandler = new JwtSecurityTokenHandler();
        var tokenResult = await jwtHandler.ValidateTokenAsync(state, new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.FromMinutes(1),
            IssuerSigningKey = securityKey,
            RequireExpirationTime = true,
            ValidateLifetime = true,
        });

        if (!tokenResult.IsValid)
        {
            // TODO: Log this case.
            throw new ArgumentException("Jwt token is not valid.", nameof(state));
        }

        var provider = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "provider").Value.ToString();

        var redirectAfter = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "redirect_after").Value.ToString();

        var flowState = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "flow_state").Value.ToString();

        if (provider is null ||
            redirectAfter is null ||
            flowState is null)
        {
            throw new ArgumentException("Jwt token is valid but structure is unknown.", nameof(state));
        }

        var targetUserId = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "target_id").Value?.ToString();

        Guid? targetId = null;
        if (!string.IsNullOrWhiteSpace(targetUserId))
        {
            if (Guid.TryParse(targetUserId, out var temp))
            {
                targetId = temp;
            }
            else
            {
                throw new ArgumentException($"Unable to parse target user id. Value: {targetUserId}");
            }
        }

        var jwtState = new State
        {
            FlowState = flowState,
            Provider = provider,
            RedirectAfter = redirectAfter,
            TargetUserId = targetId,
        };

        return jwtState;
    }
}
