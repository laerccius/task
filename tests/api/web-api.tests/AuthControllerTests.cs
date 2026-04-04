using api.application.DTOs;
using api.application.Interfaces;
using api.web_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace api.web_api.Tests;

public class AuthControllerTests
{
    private const string DefaultToken = "token";
    private const string FullNameDemoUser = "Demo User";
    private readonly Guid _defaultUserId = Guid.NewGuid();
    private readonly AutoMocker _mocker = new();
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mocker.GetMock<IUserService>()
            .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LoginRequest req, CancellationToken _) =>
                new AuthResponse(_defaultUserId, FullNameDemoUser, req.Email, DefaultToken));

        _mocker.GetMock<IUserService>()
            .Setup(service => service.RegisterAsync(It.IsAny<RegisterUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RegisterUserRequest req, CancellationToken _) =>
                new AuthResponse(_defaultUserId, req.FullName, req.Email, DefaultToken));

        _authController = _mocker.CreateInstance<AuthController>();
    }

    [Fact]
    public async Task Login_Should_Return_Ok_When_Service_Succeeds()
    {
        var request = new LoginRequest
        {
            Email = "demo@example.com",
            Password = "Password1!"
        };

        var result = await _authController.Login(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal(request.Email, payload.Email);
        Assert.Equal(DefaultToken, payload.Token);
        Assert.Equal(FullNameDemoUser, payload.FullName);
        Assert.Equal(_defaultUserId, payload.UserId);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Service_Throws_UnauthorizedAccessException()
    {
        _mocker.GetMock<IUserService>()
            .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

        var result = await _authController.Login(new LoginRequest
        {
            Email = "demo@example.com",
            Password = "wrong"
        }, CancellationToken.None);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Register_Should_Return_Ok_When_Service_Succeeds()
    {
        var request = new RegisterUserRequest
        {
            Email = "demo@example.com",
            Password = "Password1!",
            FullName = "Register User"
        };

        var result = await _authController.Register(request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal(request.Email, payload.Email);
        Assert.Equal(DefaultToken, payload.Token);
        Assert.Equal(request.FullName, payload.FullName);
        Assert.Equal(_defaultUserId, payload.UserId);
    }

    [Fact]
    public async Task Register_Should_Return_Conflict_When_Service_Throws_InvalidOperationException()
    {
        _mocker.GetMock<IUserService>()
            .Setup(service => service.RegisterAsync(It.IsAny<RegisterUserRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("A user with this email already exists."));

        var result = await _authController.Register(new RegisterUserRequest
        {
            Email = "demo@example.com",
            Password = "Password1!",
            FullName = "Register User"
        }, CancellationToken.None);

        Assert.IsType<ConflictObjectResult>(result.Result);
    }
}
