using api.application.DTOs;

namespace api.application.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskItemResponse>> GetTasksAsync(Guid userId, CancellationToken cancellationToken);
    Task<TaskItemResponse?> GetTaskByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken);
    Task<TaskItemResponse> CreateTaskAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken);
    Task<TaskItemResponse> UpdateTaskAsync(Guid taskId, Guid userId, CreateTaskRequest request, CancellationToken cancellationToken);
    Task DeleteTaskAsync(Guid taskId, Guid userId, CancellationToken cancellationToken);
}
