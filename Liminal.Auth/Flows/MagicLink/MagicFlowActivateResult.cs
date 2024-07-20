using System.Security.Claims;
using Liminal.Auth.Results;

namespace Liminal.Auth.Flows.MagicLink;

public class MagicFlowActivateResult : ResultBase
{
    public ClaimsPrincipal Principal { get; set; } = default!;
    public string Email { get; set; } = default!;

    protected MagicFlowActivateResult()
    { }

    public static MagicFlowActivateResult Failure()
    {
        return new MagicFlowActivateResult
        {
            IsSuccess = false
        };
    }
    
    public static MagicFlowActivateResult Success(ClaimsPrincipal principal, string email)
    {
        return new MagicFlowActivateResult
        {
            Principal = principal,
            Email = email,
            IsSuccess = true
        };
    }
}