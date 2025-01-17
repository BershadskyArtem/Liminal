using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Flows.OAuth;

public class StateGenerator(OAuthFlowBuilder builder) : IStateGenerator
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
            new ("flow_state", Guid.NewGuid().ToString())
        };

        if (linkingTargetId is not null)
        {
            claims.Add(new Claim("target_id", linkingTargetId.ToString()!));
        }

        var tokenOptions = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: expires,
            claims: claims);
        
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        
        return token;
    }

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
            ValidateLifetime = true
        });

        if (!tokenResult.IsValid)
        {
            // TODO: Log this case.
            throw new ArgumentException(nameof(state));
        }

        var provider = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "provider").Value.ToString();
        
        var redirectAfter = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "redirect_after").Value.ToString();
        
        var flowState = tokenResult.Claims
            .FirstOrDefault(c => c.Key == "flow_state").Value.ToString();

        if (provider is null ||
            redirectAfter is null ||
            flowState is null
           )
        {
            throw new ArgumentException(nameof(state));
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
                throw new Exception("Unable to parse target user id.");
            }
        }

        var jwtState = new State
        {
            FlowState = flowState,
            Provider = provider,
            RedirectAfter = redirectAfter,
            TargetUserId = targetId
        };

        return jwtState;
    }
}