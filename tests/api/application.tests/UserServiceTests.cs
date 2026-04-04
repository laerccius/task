using api.application.DTOs;
using api.application.Interfaces;
using api.application.Services;
using api.domain.Entities;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace api.application.Tests;

public class UserServiceTests
{
    private const string MOCK_HASH = "hash";
    private const string MOCK_SALT = "salt";
    private const string FAKE_TOKEN = "fake-token";

    private readonly AutoMocker mock = new();
    private readonly UserService userService;
    private readonly DateTime mockNow = new(2000, 10, 10, 0, 0, 0, DateTimeKind.Utc);

    public UserServiceTests()
    {
        mock.GetMock<IPasswordHasher>()
            .Setup(hasher => hasher.HashPassword(It.IsAny<string>()))
            .Returns((MOCK_HASH, MOCK_SALT));

        mock.GetMock<IDateTimeProvider>()
            .Setup(provider => provider.UtcNow)
            .Returns(mockNow);

        mock.GetMock<IUserRepository>()
            .Setup(repo => repo.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User user, CancellationToken _) => user);

        mock.GetMock<ITokenService>()
            .Setup(tokenService => tokenService.CreateToken(It.IsAny<User>()))
            .Returns(FAKE_TOKEN);

        userService = mock.CreateInstance<UserService>();
    }

    [Fact]
    public async Task RegisterAsync_Should_Create_User_When_Email_Is_New()
    {
        var request = new RegisterUserRequest
        {
            FullName = "Demo User",
            Email = "demo@example.com",
            Password = "Password1!"
        };

        var response = await userService.RegisterAsync(request, CancellationToken.None);

        Assert.Equal("demo@example.com", response.Email);
        Assert.Equal(FAKE_TOKEN, response.Token);
        mock.GetMock<IUserRepository>().Verify(repo => repo.CreateAsync(
            It.Is<User>(user =>
                user.Email == request.Email &&
                user.FullName == request.FullName &&
                user.PasswordHash == MOCK_HASH &&
                user.PasswordSalt == MOCK_SALT &&
                user.CreatedAtUtc == mockNow),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_Should_Throw_When_Email_Already_Exists()
    {
        mock.GetMock<IUserRepository>()
            .Setup(repo => repo.GetByEmailAsync("demo@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = Guid.NewGuid(), Email = "demo@example.com" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => userService.RegisterAsync(
            new RegisterUserRequest
            {
                FullName = "Demo User",
                Email = "demo@example.com",
                Password = "Password1!"
            },
            CancellationToken.None));
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "demo@example.com",
            FullName = "Demo User",
            PasswordHash = MOCK_HASH,
            PasswordSalt = MOCK_SALT
        };

        mock.GetMock<IUserRepository>()
            .Setup(repo => repo.GetByEmailAsync("demo@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mock.GetMock<IPasswordHasher>()
            .Setup(hasher => hasher.Verify("Password1!", MOCK_HASH, MOCK_SALT))
            .Returns(true);

        var response = await userService.LoginAsync(new LoginRequest
        {
            Email = "demo@example.com",
            Password = "Password1!"
        }, CancellationToken.None);

        Assert.Equal(user.Email, response.Email);
        Assert.Equal(FAKE_TOKEN, response.Token);
        mock.GetMock<ITokenService>().Verify(tokenService => tokenService.CreateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "demo@example.com",
            FullName = "Demo User",
            PasswordHash = MOCK_HASH,
            PasswordSalt = MOCK_SALT
        };

        mock.GetMock<IUserRepository>()
            .Setup(repo => repo.GetByEmailAsync("demo@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mock.GetMock<IPasswordHasher>()
            .Setup(hasher => hasher.Verify("wrong", MOCK_HASH, MOCK_SALT))
            .Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => userService.LoginAsync(
            new LoginRequest
            {
                Email = "demo@example.com",
                Password = "wrong"
            },
            CancellationToken.None));
    }
}
