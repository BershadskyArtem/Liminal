using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IAccountLinker<in TUser>
    where TUser : AbstractUser
{
    public Task<bool> LinkAccount(TUser user, Account account, bool save = false);
    public Task<bool> UnlinkAccount(TUser user, Account account, Func<TUser> userFactory, bool save = false);
}