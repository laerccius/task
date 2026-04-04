namespace api.application.DTOs;

public record AuthResponse(Guid UserId, string FullName, string Email, string Token);
