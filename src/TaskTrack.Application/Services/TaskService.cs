using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TaskService(ITaskRepository taskRepository, IDateTimeProvider dateTimeProvider)
    {
        _taskRepository = taskRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<IReadOnlyList<TaskItemResponse>> GetTasksAsync(Guid userId, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(userId, cancellationToken);
        return tasks.Select(Map).ToList();
    }

    public async Task<TaskItemResponse?> GetTaskByIdAsync(Guid taskId, Guid userId, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, userId, cancellationToken);
        return task is null ? null : Map(task);
    }

    public async Task<TaskItemResponse> CreateTaskAsync(Guid userId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        ValidateRequest(request, now);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Status = ParseStatus(request.Status),
            DueDate = request.DueDate,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        var created = await _taskRepository.CreateAsync(task, cancellationToken);
        return Map(created);
    }

    public async Task<TaskItemResponse> UpdateTaskAsync(Guid taskId, Guid userId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request, _dateTimeProvider.UtcNow);
        var existing = await _taskRepository.GetByIdAsync(taskId, userId, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        existing.Title = request.Title.Trim();
        existing.Description = request.Description.Trim();
        existing.Status = ParseStatus(request.Status);
        existing.DueDate = request.DueDate;
        existing.UpdatedAtUtc = _dateTimeProvider.UtcNow;

        var updated = await _taskRepository.UpdateAsync(existing, cancellationToken);
        if (!updated)
        {
            throw new InvalidOperationException("Unable to update task.");
        }

        return Map(existing);
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid userId, CancellationToken cancellationToken)
    {
        var deleted = await _taskRepository.DeleteAsync(taskId, userId, cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException("Task not found.");
        }
    }

    private static void ValidateRequest(CreateTaskRequest request, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (request.Title.Trim().Length > 120)
        {
            throw new ArgumentException("Title must be 120 characters or fewer.");
        }

        if (request.DueDate == default)
        {
            throw new ArgumentException("Due date is required.");
        }

        if (request.DueDate < now.Date.AddDays(-1))
        {
            throw new ArgumentException("Due date must not be far in the past.");
        }
    }

    private static TaskItemStatus ParseStatus(int status)
    {
        if (!Enum.IsDefined(typeof(TaskItemStatus), status))
        {
            throw new ArgumentException("Invalid task status.");
        }

        return (TaskItemStatus)status;
    }

    private static TaskItemResponse Map(TaskItem task) =>
        new(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.DueDate,
            task.CreatedAtUtc,
            task.UpdatedAtUtc);
}
