using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Results;

namespace Liminal.Auth.Jwt;

public class JwtTokenGenerator(JwtTokenGeneratorOptions options) : ITokenGenerator
{
    public string Name { get; } = "jwt";

    protected readonly JwtTokenGeneratorOptions Options = options;

    public Task<GenerateTokenResult> GenerateToken(ClaimsPrincipal principal)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshTokenResult> RefreshToken(string refreshToken)
    {
        throw new NotImplementedException();
    }
}