using api.application.Interfaces;
using api.domain.Entities;
using api.domain.Enums;
using api.infrastructure.Auth;
using api.infrastructure.Data;
using api.infrastructure.Repositories;
using Xunit;

namespace api.infrastructure.Tests;

public class TaskRepositoryTests
{
    [Fact]
    public async Task CreateAsync_And_GetByUserIdAsync_Should_Persist_Task()
    {
        var (repository, keepAlive, dateTimeProvider) = await CreateRepositoryAsync();
        await using var _ = keepAlive;
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var task = CreateTask(userId, dateTimeProvider.UtcNow);

        await repository.CreateAsync(task, CancellationToken.None);
        var results = await repository.GetByUserIdAsync(userId, CancellationToken.None);

        Assert.Contains(results, x => x.Id == task.Id);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_For_Different_User()
    {
        var (repository, keepAlive, dateTimeProvider) = await CreateRepositoryAsync();
        await using var _ = keepAlive;
        var ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var otherUserId = Guid.NewGuid();
        var task = CreateTask(ownerId, dateTimeProvider.UtcNow);

        await repository.CreateAsync(task, CancellationToken.None);
        var result = await repository.GetByIdAsync(task.Id, otherUserId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Task_Fields()
    {
        var (repository, keepAlive, dateTimeProvider) = await CreateRepositoryAsync();
        await using var _ = keepAlive;
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var task = CreateTask(userId, dateTimeProvider.UtcNow);

        await repository.CreateAsync(task, CancellationToken.None);

        task.Title = "Updated title";
        task.Description = "Updated description";
        task.Status = TaskItemStatus.Completed;
        task.UpdatedAtUtc = dateTimeProvider.UtcNow.AddHours(1);

        var updated = await repository.UpdateAsync(task, CancellationToken.None);
        var stored = await repository.GetByIdAsync(task.Id, userId, CancellationToken.None);

        Assert.True(updated);
        Assert.NotNull(stored);
        Assert.Equal("Updated title", stored!.Title);
        Assert.Equal(TaskItemStatus.Completed, stored.Status);
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Task()
    {
        var (repository, keepAlive, dateTimeProvider) = await CreateRepositoryAsync();
        await using var _ = keepAlive;
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var task = CreateTask(userId, dateTimeProvider.UtcNow);

        await repository.CreateAsync(task, CancellationToken.None);

        var deleted = await repository.DeleteAsync(task.Id, userId, CancellationToken.None);
        var stored = await repository.GetByIdAsync(task.Id, userId, CancellationToken.None);

        Assert.True(deleted);
        Assert.Null(stored);
    }

    private static async Task<(TaskRepository Repository, Microsoft.Data.Sqlite.SqliteConnection KeepAlive, FakeDateTimeProvider DateTimeProvider)> CreateRepositoryAsync()
    {
        var databaseName = $"tasktrack-{Guid.NewGuid()}";
        var factory = new SqliteConnectionFactory($"Data Source={databaseName};Mode=Memory;Cache=Shared");
        var keepAlive = factory.CreateConnection();
        await keepAlive.OpenAsync();

        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc));
        var initializer = new SqliteDbInitializer(factory, new Pbkdf2PasswordHasher(), dateTimeProvider);
        await initializer.InitializeAsync();

        return (new TaskRepository(factory), keepAlive, dateTimeProvider);
    }

    private static TaskItem CreateTask(Guid userId, DateTime now) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "Write tests",
            Description = "Cover repository behavior",
            Status = TaskItemStatus.Pending,
            DueDate = now.AddDays(1),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
