using api.application.DTOs;
using api.application.Interfaces;
using api.application.Services;
using api.domain.Entities;
using api.domain.Enums;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace api.application.Tests;

public class TaskServiceTests
{
    private readonly AutoMocker mock = new();
    private readonly DateTime mockNow = new(2000, 10, 10, 0, 0, 0, DateTimeKind.Utc);
    private readonly TaskService taskService;

    public TaskServiceTests()
    {
        mock.GetMock<IDateTimeProvider>()
            .Setup(provider => provider.UtcNow)
            .Returns(mockNow);

        mock.GetMock<ITaskRepository>()
            .Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem taskItem, CancellationToken _) => taskItem);

        mock.GetMock<ITaskRepository>()
            .Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        taskService = mock.CreateInstance<TaskService>();
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Create_Task_When_Request_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var requestTaskItemStatus = TaskItemStatus.Pending;
        var request = new CreateTaskRequest
        {
            Title = "Prepare deck",
            Description = "Finalize architecture slides",
            Status = (int)requestTaskItemStatus,
            DueDate = mockNow.AddDays(1)
        };

        var response = await taskService.CreateTaskAsync(userId, request, CancellationToken.None);

        Assert.Equal(request.Title, response.Title);
        Assert.Equal(request.Description, response.Description);
        Assert.Equal(requestTaskItemStatus.ToString(), response.Status);
        Assert.Equal(request.DueDate, response.DueDate);

        mock.GetMock<ITaskRepository>().Verify(repo => repo.CreateAsync(
            It.Is<TaskItem>(taskItem =>
                taskItem.UserId == userId &&
                taskItem.Title == request.Title &&
                taskItem.Description == request.Description &&
                taskItem.DueDate == request.DueDate &&
                taskItem.Status == requestTaskItemStatus &&
                taskItem.CreatedAtUtc == mockNow &&
                taskItem.UpdatedAtUtc == mockNow),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Reject_Empty_Title()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => taskService.CreateTaskAsync(
            Guid.NewGuid(),
            new CreateTaskRequest
            {
                Title = " ",
                Description = "desc",
                Status = (int)TaskItemStatus.Pending,
                DueDate = mockNow.AddDays(1)
            },
            CancellationToken.None));
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Reject_Invalid_Status()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => taskService.CreateTaskAsync(
            Guid.NewGuid(),
            new CreateTaskRequest
            {
                Title = "Task",
                Description = "desc",
                Status = 99,
                DueDate = mockNow.AddDays(1)
            },
            CancellationToken.None));
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Update_When_Task_Exists()
    {
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existing = new TaskItem
        {
            Id = taskId,
            UserId = userId,
            Title = "Old title",
            Description = "Old description",
            Status = TaskItemStatus.Pending,
            DueDate = mockNow.AddDays(1),
            CreatedAtUtc = mockNow.AddDays(-1),
            UpdatedAtUtc = mockNow.AddDays(-1)
        };

        mock.GetMock<ITaskRepository>()
            .Setup(repo => repo.GetByIdAsync(taskId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var request = new CreateTaskRequest
        {
            Title = "Updated title",
            Description = "Updated description",
            Status = (int)TaskItemStatus.Completed,
            DueDate = mockNow.AddDays(3)
        };

        var response = await taskService.UpdateTaskAsync(taskId, userId, request, CancellationToken.None);

        Assert.Equal(request.Title, response.Title);
        Assert.Equal(request.Description, response.Description);
        Assert.Equal(TaskItemStatus.Completed.ToString(), response.Status);
        Assert.Equal(request.DueDate, response.DueDate);

        mock.GetMock<ITaskRepository>().Verify(repo => repo.UpdateAsync(
            It.Is<TaskItem>(taskItem =>
                taskItem.Id == taskId &&
                taskItem.UserId == userId &&
                taskItem.Title == request.Title &&
                taskItem.Description == request.Description &&
                taskItem.Status == TaskItemStatus.Completed &&
                taskItem.DueDate == request.DueDate &&
                taskItem.UpdatedAtUtc == mockNow),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_Should_Throw_When_Task_Does_Not_Exist()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => taskService.UpdateTaskAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new CreateTaskRequest
            {
                Title = "Updated",
                Description = "desc",
                Status = (int)TaskItemStatus.Completed,
                DueDate = mockNow.AddDays(2)
            },
            CancellationToken.None));
    }

    [Fact]
    public async Task DeleteTaskAsync_Should_Throw_When_Task_Does_Not_Exist()
    {
        mock.GetMock<ITaskRepository>()
            .Setup(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            taskService.DeleteTaskAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None));
    }
}
