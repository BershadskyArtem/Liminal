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
    private const string Scheme = "Fake";
    private const string EncodedState = "abcd";
    private const string Code = "thisisoauthcode";
    private const string AccessToken = "thisisoauthaccess";
    private const string Email1 = "email1@test.com";

    private static readonly AbstractUser User1 = new()
    {
        Email = Email1,
        Id = Guid.NewGuid()
    };

    private static readonly State StateWithTargetUser = new()
    {
        Provider = Scheme,
        RedirectAfter = "/",
        TargetUserId = Guid.NewGuid()
    };
    
    private static readonly State StateWithoutTargetUser = new()
    {
        Provider = Scheme,
        RedirectAfter = "/",
        TargetUserId = null
    };
    
    private static readonly Account Account1 = Account.CreateConfirmed(Scheme, Email1, User1.Id);
    private static readonly OAuthSignInResult SuccessEmail1 = OAuthSignInResult
        .Success(
            TokenSet.Create(AccessToken, DateTimeOffset.UtcNow), 
            UserInfo.Create(Email1, Email1, true), Scheme);
    
    private readonly OAuthFlow<AbstractUser> _sut;
    private readonly IOAuthProvider _provider = Substitute.For<IOAuthProvider>();
    private readonly IOAuthProvidersProvider _providers = Substitute.For<IOAuthProvidersProvider>();
    private readonly IStateGenerator _stateGenerator = Substitute.For<IStateGenerator>();
    private readonly IUserStore<AbstractUser> _userStore = Substitute.For<IUserStore<AbstractUser>>();
    private readonly IPasswordStore _passwordStore = Substitute.For<IPasswordStore>();
    private readonly IAccountStore _accountStore = Substitute.For<IAccountStore>();
    private readonly IUserFactory<AbstractUser> _userFactory = new DefaultUserFactory<AbstractUser>();
    
    public OAuthFlowTests()
    {
        _sut = new OAuthFlow<AbstractUser>(
            _providers,
            _stateGenerator,
            _userFactory,
            _userStore,
            _passwordStore,
            _accountStore);
    }

    [Fact]
    public async Task Callback_ShouldSucceedAndCreateAccountAndUser_WhenAccountAndUserDoNotExist()
    {
        _stateGenerator
            .ParseState(EncodedState)
            .Returns(StateWithoutTargetUser);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore
            .AddAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore
            .GetByEmailAsync(Email1)
            .Returns(Task.FromResult<AbstractUser?>(null));

        AbstractUser? createdUser = null;
        
        _userStore
            .AddAsync(Arg.Any<AbstractUser>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdUser = info.Args().First() as AbstractUser;
            });
        
        // Act
        await _sut.Callback(Scheme, Code, EncodedState);
        
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
        _stateGenerator
            .ParseState(EncodedState)
            .Returns(StateWithTargetUser);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore
            .UpdateAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore
            .GetByIdAsync(StateWithTargetUser.TargetUserId!.Value)
            .Returns(Task.FromResult<AbstractUser?>(null));

        _userStore
            .GetByEmailAsync(Email1)
            .Returns(Task.FromResult<AbstractUser?>(null));

        AbstractUser? createdUser = null;
        
        _userStore
            .AddAsync(Arg.Any<AbstractUser>(), true)
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

        await call
            .Should()
            .ThrowAsync<Exception>();
        
        // Assert
        Assert.Null(createdAccount);
        Assert.Null(createdUser);
    }
    
    [Fact]
    public async Task Callback_ShouldSucceed_WhenAccountDoesNotExistAndTargetUserExistsWithMatchingEmailsConfirmed()
    {
        _stateGenerator
            .ParseState(EncodedState)
            .Returns(StateWithTargetUser);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore
            .AddAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore
            .GetByIdAsync(StateWithTargetUser.TargetUserId!.Value)
            .Returns(Task.FromResult<AbstractUser?>(User1));

       // _userStore.GetByEmailAsync(Email1).Returns(Task.FromResult<AbstractUser?>(null));

        User1.Confirm();
        // Act
        await _sut.Callback(Scheme, Code, EncodedState);
        
        // Assert
        Assert.NotNull(createdAccount);
        Assert.Equal(createdAccount.UserId, User1.Id);
    }
    
    [Fact]
    public async Task Callback_ShouldFail_WhenAccountDoesNotExistAndTargetUserExistsWithMatchingEmailsNotConfirmed()
    {
        // Assert
        var state = new State
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = Guid.NewGuid()
        };
        
        _stateGenerator
            .ParseState(EncodedState).Returns(state);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(null));

        Account? createdAccount = null;
        _accountStore
            .AddAsync(Arg.Any<Account>(), true)
            .Returns(true)
            .AndDoes(info =>
            {
                createdAccount = info.Args().First() as Account;
            });

        _userStore
            .GetByIdAsync(state.TargetUserId.Value)
            .Returns(Task.FromResult<AbstractUser?>(User1));

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
        var state = new State
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = null
        };
        
        _stateGenerator
            .ParseState(EncodedState)
            .Returns(state);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(Account1));

        _userStore
            .GetByIdAsync(User1.Id)
            .Returns(Task.FromResult<AbstractUser?>(User1));
        
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
        var state = new State
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = User1.Id
        };
        
        _stateGenerator
            .ParseState(EncodedState)
            .Returns(state);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(Account1));

        _userStore
            .GetByIdAsync(User1.Id)
            .Returns(Task.FromResult<AbstractUser?>(User1));
        
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
        var state = new State
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = null
        };
        
        _stateGenerator
            .ParseState(EncodedState).Returns(state);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(Account1));

        _userStore
            .GetByIdAsync(User1.Id)
            .Returns(Task.FromResult<AbstractUser?>(User1));
        
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
        var state = new State
        {
            Provider = Scheme,
            RedirectAfter = "/",
            TargetUserId = User1.Id
        };
        
        _stateGenerator
            .ParseState(EncodedState)
            .Returns(state);
        
        _provider
            .SignInOAuthAsync(Code, EncodedState)
            .Returns(Task.FromResult(SuccessEmail1));
        
        _providers
            .GetProvider(Scheme)
            .Returns(_provider);

        _accountStore
            .GetByProviderAsync(Email1, Scheme)
            .Returns(Task.FromResult<Account?>(Account1));

        _userStore
            .GetByIdAsync(User1.Id)
            .Returns(Task.FromResult<AbstractUser?>(User1));
        
        User1.UnConfirm();
        // Act
        var result = async () =>
        {
            await _sut.Callback(Scheme, Code, EncodedState);
        };
        
        // Assert
        await result
            .Should()
            .ThrowAsync<Exception>();
    }
}