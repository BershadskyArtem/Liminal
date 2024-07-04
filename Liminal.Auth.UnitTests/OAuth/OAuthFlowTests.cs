using System.Security.Claims;
using FluentAssertions;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Flows.OAuth.Providers;
using Liminal.Auth.Implementations;
using Liminal.Auth.Models;
using NSubstitute;
using Xunit;
using Assert = Xunit.Assert;

namespace Liminal.Auth.UnitTests.OAuth;

public class OAuthFlowTests
{
    public static readonly string Scheme = "Fake";
    public static readonly string EncodedState = "abcd";
    public static readonly string Code = "thisisoauthcode";
    public static readonly List<Claim> Claims = new List<Claim>();
    public static readonly string AccessToken = "thisisoauthaccess";
    public static readonly string RefreshToken = "thisisoauthrefresh";
    public static readonly string Email1 = "email1@test.com";
    public static readonly string Email2 = "email2@test.com";

    public static readonly AbstractUser User1 = new AbstractUser()
    {
        Email = Email1,
        Id = Guid.NewGuid(),
    };
    
    public static readonly AbstractUser User2 = new AbstractUser()
    {
        Email = Email2,
        Id = Guid.NewGuid(),
    };

    public static readonly Account Account1 = Account.CreateConfirmed(Scheme, Email1, User1.Id);
    
    private readonly OAuthFlow<AbstractUser> _sut;
    private readonly IOAuthProvider _provider = Substitute.For<IOAuthProvider>();
    private readonly IOAuthProvidersProvider _proivders = Substitute.For<IOAuthProvidersProvider>();
    private readonly IStateGenerator _stateGenerator = Substitute.For<IStateGenerator>();
    private readonly IUserStore<AbstractUser> _userStore = Substitute.For<IUserStore<AbstractUser>>();
    private readonly IPasswordStore _passwordStore = Substitute.For<IPasswordStore>();
    private readonly IAccountStore _accountStore = Substitute.For<IAccountStore>();
    private readonly IUserFactory<AbstractUser> _userFactory = new DefaultUserFactory<AbstractUser>();
    
    
    public OAuthFlowTests()
    {
        _sut = new OAuthFlow<AbstractUser>(
            _proivders,
            _stateGenerator,
            _userFactory,
            _userStore,
            _passwordStore,
            _accountStore);

    }

    [Fact]
    public async Task Callback_ShouldSucceedAndCreateAccountAndUser_WhenAccountAndUserDoNotExist()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = null,
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore.AddAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore.GetByEmailAsync(Email1).Returns(Task.FromResult<AbstractUser?>(null));

        AbstractUser? createdUser = null;
        
        _userStore.AddAsync(Arg.Any<AbstractUser>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdUser = info.Args().First() as AbstractUser;
            });
        
        // Act
        var result = await _sut.Callback(Scheme, Code, EncodedState);
        
        // Assert
        Assert.NotNull(createdAccount);
        Assert.NotNull(createdUser);
        Assert.Equal(createdAccount.UserId, createdUser.Id);
        Assert.True(createdAccount.IsConfirmed);
        Assert.True(createdUser.IsConfirmed);
    }
    
    [Fact]
    public async Task Callback_ShouldThrow_WhenAccountAndUserDoNotExistAndTargetUserDoesNotExist()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = Guid.NewGuid(),
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore.UpdateAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore.GetByIdAsync(state.TargetUserId.Value).Returns(Task.FromResult<AbstractUser?>(null));

        _userStore.GetByEmailAsync(Email1).Returns(Task.FromResult<AbstractUser?>(null));

        AbstractUser? createdUser = null;
        
        _userStore.AddAsync(Arg.Any<AbstractUser>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdUser = info.Args().First() as AbstractUser;
            });
        
        // Act
        var call = async () =>
        {   
            await _sut.Callback(Scheme, Code, EncodedState);
        };

        await call.Should().ThrowAsync<Exception>();
        
        // Assert
        Assert.Null(createdAccount);
        Assert.Null(createdUser);
    }
    
    [Fact]
    public async Task Callback_ShouldSucceed_WhenAccountDoesNotExistAndTargetUserExistsWithMatchingEmailsConfirmed()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = Guid.NewGuid(),
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore.AddAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore.GetByIdAsync(state.TargetUserId.Value).Returns(Task.FromResult<AbstractUser?>(User1));

       // _userStore.GetByEmailAsync(Email1).Returns(Task.FromResult<AbstractUser?>(null));

        User1.Confirm();
        // Act
        var result = await _sut.Callback(Scheme, Code, EncodedState);
        
        // Assert
        Assert.NotNull(createdAccount);
        Assert.Equal(createdAccount.UserId, User1.Id);
    }
    
    [Fact]
    public async Task Callback_ShouldFail_WhenAccountDoesNotExistAndTargetUserExistsWithMatchingEmailsNotConfirmed()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = Guid.NewGuid(),
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore.AddAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore.GetByIdAsync(state.TargetUserId.Value).Returns(Task.FromResult<AbstractUser?>(User1));

        // _userStore.GetByEmailAsync(Email1).Returns(Task.FromResult<AbstractUser?>(null));

        User1.UnConfirm();
        // Act
        var result = async () =>
        {
            await _sut.Callback(Scheme, Code, EncodedState);
        };

        await result.Should().ThrowAsync<Exception>();
        
        // Assert
        Assert.Null(createdAccount);
    }
    
    [Fact]
    public async Task Callback_ShouldSucceed_WhenLinkedAccountAndUserExistConfirmedNoTarget()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = null,
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(Account1));

        _userStore.GetByIdAsync(User1.Id).Returns(Task.FromResult<AbstractUser?>(User1));
        
        User1.Confirm();
        // Act
        var result = await _sut.Callback(Scheme, Code, EncodedState);
        
        // Assert
        result.IsSuccess.Should().Be(true);
        var claims = User1.ToPrincipal().Claims;
        foreach (var claim in claims)
        {
            var otherClaim = result.Principal!.Claims.First(c => c.Type == claim.Type);

            Assert.Equal(otherClaim.Value, claim.Value);
        }
    }
    
    [Fact]
    public async Task Callback_ShouldSucceed_WhenLinkedAccountAndUserExistConfirmedWithTarget()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = User1.Id,
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(Account1));

        _userStore.GetByIdAsync(User1.Id).Returns(Task.FromResult<AbstractUser?>(User1));
        
        User1.Confirm();
        // Act
        var result = await _sut.Callback(Scheme, Code, EncodedState);
        
        // Assert
        result.IsSuccess.Should().Be(true);
        var claims = User1.ToPrincipal().Claims;
        foreach (var claim in claims)
        {
            var otherClaim = result.Principal!.Claims.First(c => c.Type == claim.Type);

            Assert.Equal(otherClaim.Value, claim.Value);
        }
    }
    
    [Fact]
    public async Task Callback_ShouldSucceedAndConfirm_WhenLinkedAccountAndUserExistNotConfirmedNoTarget()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = null,
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(Account1));

        _userStore.GetByIdAsync(User1.Id).Returns(Task.FromResult<AbstractUser?>(User1));
        
        User1.UnConfirm();
        // Act
        var result = await _sut.Callback(Scheme, Code, EncodedState);
        
        // Assert
        result.IsSuccess.Should().Be(true);
        var claims = User1.ToPrincipal().Claims;
        foreach (var claim in claims)
        {
            var otherClaim = result.Principal!.Claims.First(c => c.Type == claim.Type);

            Assert.Equal(otherClaim.Value, claim.Value);
        }

        User1.IsConfirmed.Should().Be(true);
    }
    
    [Fact]
    public async Task Callback_ShouldThrow_WhenLinkedAccountAndUserExistNotConfirmedWithValidTarget()
    {
        // Assert
        var state = new State()
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = User1.Id,
        };
        
        _stateGenerator.ParseState(EncodedState).Returns(state);
        _provider.SignInOAuthAsync(Code, EncodedState).Returns(
            Task.FromResult(
                OAuthSignInResult.Success(TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), UserInfo.Create(Email1, Email1, true), Scheme )
            ));
        _proivders.GetProvider(Scheme).Returns(_provider);

        _accountStore.GetByProviderAsync(Email1, Scheme)
            .Returns(
                Task.FromResult<Account?>(Account1));

        _userStore.GetByIdAsync(User1.Id).Returns(Task.FromResult<AbstractUser?>(User1));
        
        User1.UnConfirm();
        // Act
        var result = async () =>
        {
            await _sut.Callback(Scheme, Code, EncodedState);
        };
        
        // Assert
        await result.Should().ThrowAsync<Exception>();
    }
}