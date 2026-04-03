using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Interfaces;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}
