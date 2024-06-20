using System.Security.Claims;

namespace Liminal.Auth.Models;

public class UserClaim
{
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string ClaimType { get; set; }
    public virtual string ClaimValue { get; set; }

    public Claim ToClaim()
    {
        return new Claim(ClaimType, ClaimValue);
    }

    public static UserClaim FromClaim(Claim claim)
    {
        return new UserClaim()
        {
            ClaimType = claim.Type,
            ClaimValue = claim.Value
        };
    }
}