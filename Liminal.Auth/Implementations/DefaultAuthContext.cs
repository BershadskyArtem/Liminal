using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;
using Microsoft.AspNetCore.Http;

namespace Liminal.Auth.Implementations;

public class DefaultAuthContext<TUser> : IAuthContext<TUser> 
    where TUser : AbstractUser
{
    public DefaultAuthContext(IHttpContextAccessor accessor, IUserStore<TUser> userStore)
    {
        _userStore = userStore;
        UserId = null;
        IsConfirmed = false;

        var context = accessor?.HttpContext;

        if (context is null)
        {
            return;
        }
        
        var idClaim = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (!string.IsNullOrWhiteSpace(idClaim) && Guid.TryParse(idClaim, out var id))
        {
            UserId = id;
        }

        var confirmedClaim = context.User.Claims.FirstOrDefault(c => c.Type == "confirmed")?.Value;
        if (!string.IsNullOrWhiteSpace(confirmedClaim) && bool.TryParse(confirmedClaim, out var confirmed))
        {
            IsConfirmed = confirmed;
        }
    }

    public Guid? UserId { get; set; }
    public bool IsConfirmed { get; set; }
    public async Task<string?> Role()
    {
        return (await Current())?.Role;
    }

    public async Task<TUser?> Current()
    {
        if (UserId is null)
        {
            return null;
        }
        
        if (_user is not null)
        {
            return _user;
        }

        _user = await _userStore.GetByIdAsync(UserId.Value);

        if (_user is null)
        {
            UserId = null;
        }

        return _user;
    }
    
    private readonly IUserStore<TUser> _userStore;
    private TUser? _user;
}