using Microsoft.AspNetCore.Mvc;
using api.application.DTOs;
using api.application.Interfaces;
using api.web_api.Controllers;
using Xunit;
using Moq.AutoMock;
using Moq;

namespace api.web_api.Tests;

public class AuthControllerTests
{


    private AutoMocker mock = new AutoMocker();
    private readonly AuthController _authController;
    private const string DEFAULT_TOKEN = "token";
    private const string FULL_NAME_DEMO_USER = "Demo User";
    private Guid DEFAULT_USER_ID = Guid.NewGuid();

    public AuthControllerTests()
    {
        mock.GetMock<IUserService>()
        .Setup(service => service.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((application.DTOs.LoginRequest req, CancellationToken ct) => new AuthResponse(DEFAULT_USER_ID, FULL_NAME_DEMO_USER, req.Email, DEFAULT_TOKEN));
        mock.GetMock<IUserService>()
        .Setup(service => service.RegisterAsync(It.IsAny<RegisterUserRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((RegisterUserRequest req, CancellationToken ct) => new AuthResponse(DEFAULT_USER_ID, req.FullName, req.Email, DEFAULT_TOKEN));

        _authController = mock.CreateInstance<AuthController>();
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
        Assert.Equal(DEFAULT_TOKEN, payload.Token);
        Assert.Equal(FULL_NAME_DEMO_USER, payload.FullName);
        Assert.Equal(DEFAULT_USER_ID, payload.UserId);
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
        Assert.Equal(DEFAULT_TOKEN, payload.Token);
        Assert.Equal(request.FullName, payload.FullName);
        Assert.Equal(DEFAULT_USER_ID, payload.UserId);
    }
}
