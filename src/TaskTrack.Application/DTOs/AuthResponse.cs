namespace TaskTrack.Application.DTOs;

public record AuthResponse(Guid UserId, string FullName, string Email, string Token);
