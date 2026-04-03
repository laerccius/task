using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Services;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using Xunit;

namespace TaskTrack.Application.Tests;

public class TaskServiceTests
{
    [Fact]
    public async Task CreateTaskAsync_Should_Create_Task_When_Request_Is_Valid()
    {
        var repository = new InMemoryTaskRepository();
        var now = new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc);
        var service = new TaskService(repository, new FakeDateTimeProvider(now));
        var userId = Guid.NewGuid();

        var response = await service.CreateTaskAsync(userId, new CreateTaskRequest
        {
            Title = "Prepare deck",
            Description = "Finalize architecture slides",
            Status = (int)TaskItemStatus.Pending,
            DueDate = now.AddDays(1)
        }, CancellationToken.None);

        Assert.Equal("Prepare deck", response.Title);
        Assert.Single(repository.Items);
        Assert.Equal(userId, repository.Items[0].UserId);
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Reject_Empty_Title()
    {
        var service = new TaskService(new InMemoryTaskRepository(), new FakeDateTimeProvider(DateTime.UtcNow));

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTaskAsync(
            Guid.NewGuid(),
            new CreateTaskRequest
            {
                Title = " ",
                Description = "desc",
                Status = (int)TaskItemStatus.Pending,
                DueDate = DateTime.UtcNow.AddDays(1)
            },
            CancellationToken.None));
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Throw_When_Task_Does_Not_Exist()
    {
        var service = new TaskService(new InMemoryTaskRepository(), new FakeDateTimeProvider(DateTime.UtcNow));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateTaskAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new CreateTaskRequest
            {
                Title = "Updated",
                Description = "desc",
                Status = (int)TaskItemStatus.Completed,
                DueDate = DateTime.UtcNow.AddDays(2)
            },
            CancellationToken.None));
    }

    private sealed class InMemoryTaskRepository : ITaskRepository
    {
        public List<TaskItem> Items { get; } = [];

        public Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            Items.Add(taskItem);
            return Task.FromResult(taskItem);
        }

        public Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var item = Items.FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (item is null)
            {
                return Task.FromResult(false);
            }

            Items.Remove(item);
            return Task.FromResult(true);
        }

        public Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.FirstOrDefault(x => x.Id == id && x.UserId == userId));
        }

        public Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<TaskItem>>(Items.Where(x => x.UserId == userId).ToList());
        }

        public Task<bool> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            var index = Items.FindIndex(x => x.Id == taskItem.Id && x.UserId == taskItem.UserId);
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            Items[index] = taskItem;
            return Task.FromResult(true);
        }
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
