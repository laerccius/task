using api.application.DTOs;
using api.application.Interfaces;
using api.application.Services;
using api.domain.Entities;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace api.application.Tests;

public partial class UserServiceTests
{

    private AutoMocker mock = new AutoMocker();
    private readonly UserService _userService;
    private DateTime _mockNow = new DateTime(2000, 10, 10);
    private const string MOCK_HASH = "hash";
    private const string MOCK_SALT = "salt";
    private const string FAKE_TOKEN = "fake-token";

    public UserServiceTests()
    {

        mock.GetMock<IPasswordHasher>().Setup(hasher => hasher.HashPassword(It.IsAny<string>())).Returns((MOCK_HASH, MOCK_SALT));
        mock.GetMock<IPasswordHasher>().Setup(hasher => hasher.Verify(It.Is<string>(v => v == "wrong"), It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        mock.GetMock<IDateTimeProvider>().Setup(provider => provider.UtcNow).Returns(_mockNow);
        mock.GetMock<IUserRepository>().Setup(repo=> repo.CreateAsync(It.IsAny<User>(),It.IsAny<CancellationToken>())).ReturnsAsync((User user, CancellationToken ct) => user);
        mock.GetMock<ITokenService>().Setup(repo=> repo.CreateToken(It.IsAny<User>())).Returns(FAKE_TOKEN);
        _userService = mock.CreateInstance<UserService>();

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
        var response = await _userService.RegisterAsync(request, CancellationToken.None);

        Assert.Equal("demo@example.com", response.Email);
        Assert.Equal("fake-token", response.Token);
        mock.GetMock<IUserRepository>().Verify(rep => rep.CreateAsync(It.Is<User>(user => user.Email == request.Email
                                                          && user.FullName == request.FullName
                                                          && user.PasswordHash == MOCK_HASH
                                                          && user.PasswordSalt == MOCK_SALT
                                                          && user.CreatedAtUtc == _mockNow), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
    {

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.LoginAsync(
            new LoginRequest
            {
                Email = "demo@example.com",
                Password = "wrong"
            },
            CancellationToken.None));
    }

}
