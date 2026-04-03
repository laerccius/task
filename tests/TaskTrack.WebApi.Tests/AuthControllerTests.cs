using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.WebApi.Controllers;
using Xunit;

namespace TaskTrack.WebApi.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_Should_Return_Ok_When_Service_Succeeds()
    {
        var controller = new AuthController(new StubUserService());

        var result = await controller.Login(new LoginRequest
        {
            Email = "demo@example.com",
            Password = "Password1!"
        }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("demo@example.com", payload.Email);
    }

    private sealed class StubUserService : IUserService
    {
        public Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AuthResponse(Guid.NewGuid(), "Demo User", request.Email, "token"));
        }

        public Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AuthResponse(Guid.NewGuid(), request.FullName, request.Email, "token"));
        }
    }
}
