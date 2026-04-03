using TaskTrack.Application.Interfaces;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using TaskTrack.Infrastructure.Auth;
using TaskTrack.Infrastructure.Data;
using TaskTrack.Infrastructure.Repositories;
using Xunit;

namespace TaskTrack.Infrastructure.Tests;

public class TaskRepositoryTests
{
    [Fact]
    public async Task CreateAsync_And_GetByUserIdAsync_Should_Persist_Task()
    {
        var databaseName = $"tasktrack-{Guid.NewGuid()}";
        var factory = new SqliteConnectionFactory($"Data Source={databaseName};Mode=Memory;Cache=Shared");
        await using var keepAlive = factory.CreateConnection();
        await keepAlive.OpenAsync();

        var dateTimeProvider = new FakeDateTimeProvider(new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc));
        var initializer = new SqliteDbInitializer(factory, new Pbkdf2PasswordHasher(), dateTimeProvider);
        await initializer.InitializeAsync();

        var repository = new TaskRepository(factory);
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = "Write tests",
            Description = "Cover repository behavior",
            Status = TaskItemStatus.Pending,
            DueDate = dateTimeProvider.UtcNow.AddDays(1),
            CreatedAtUtc = dateTimeProvider.UtcNow,
            UpdatedAtUtc = dateTimeProvider.UtcNow
        };

        await repository.CreateAsync(task, CancellationToken.None);
        var results = await repository.GetByUserIdAsync(userId, CancellationToken.None);

        Assert.Contains(results, x => x.Id == task.Id);
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
