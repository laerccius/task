namespace api.application.DTOs;

public record TaskItemResponse(
    Guid Id,
    string Title,
    string Description,
    string Status,
    DateTime DueDate,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
