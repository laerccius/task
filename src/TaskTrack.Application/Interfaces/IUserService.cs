using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
