using api.application.DTOs;
using api.application.Interfaces;
using api.application.Services;
using api.domain.Entities;
using api.domain.Enums;
using Microsoft.VisualBasic;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace api.application.Tests;

public partial class TaskServiceTests
{

    private AutoMocker mock = new AutoMocker();
    private DateTime _mockNow = new DateTime(2000, 10, 10);
    private readonly TaskService _taskService;
    public TaskServiceTests()
    {
        mock.GetMock<ITaskRepository>().Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskItem taskItem, CancellationToken ct) => taskItem);
        mock.GetMock<IDateTimeProvider>().Setup(provider => provider.UtcNow).Returns(_mockNow);
        _taskService = mock.CreateInstance<TaskService>();
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
            DueDate = _mockNow.AddDays(1)
        };


        var response = await _taskService.CreateTaskAsync(userId, request, CancellationToken.None);

        Assert.Equal(request.Title, response.Title);
        Assert.Equal(request.Description, response.Description);
        Assert.Equal(requestTaskItemStatus.ToString(), response.Status);
        Assert.Equal(request.DueDate, response.DueDate);

        mock.GetMock<ITaskRepository>().Verify(rep => rep.CreateAsync(It.Is<TaskItem>(taskItem => taskItem.UserId == userId
                                                            && taskItem.Description == request.Description
                                                            && taskItem.DueDate == request.DueDate
                                                            && taskItem.Status == requestTaskItemStatus
                                                            && taskItem.CreatedAtUtc == _mockNow
                                                            && taskItem.UpdatedAtUtc == _mockNow), It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Reject_Empty_Title()
    {

        await Assert.ThrowsAsync<ArgumentException>(() => _taskService.CreateTaskAsync(
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

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.UpdateTaskAsync(
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
}
