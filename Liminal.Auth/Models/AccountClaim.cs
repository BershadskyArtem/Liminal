using System.Security.Claims;

namespace Liminal.Auth.Models;

public class AccountClaim
{
    public virtual Guid Id { get; set; }
    public virtual Guid AccountId { get; set; }
    public virtual string ClaimType { get; set; }
    public virtual string ClaimValue { get; set; }

    public Claim ToClaim()
    {
        return new Claim(ClaimType, ClaimValue);
    }

    public static AccountClaim FromClaim(Claim claim, Guid userId)
    {
        return new AccountClaim
        {
            ClaimType = claim.Type,
            ClaimValue = claim.Value,
            AccountId = userId,
            Id = Guid.NewGuid()
        };
    }
}