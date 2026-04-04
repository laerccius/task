using System.Security.Claims;
using api.application.DTOs;
using api.application.Interfaces;
using api.web_api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api.web_api.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _taskService = new();
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public async Task CreateTask_Should_Return_BadRequest_When_Service_Throws_ArgumentException()
    {
        _taskService
            .Setup(service => service.CreateTaskAsync(It.IsAny<Guid>(), It.IsAny<CreateTaskRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Title is required."));

        var controller = CreateController();

        var result = await controller.CreateTask(new CreateTaskRequest(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteTask_Should_Return_NotFound_When_Service_Throws_KeyNotFoundException()
    {
        _taskService
            .Setup(service => service.DeleteTaskAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Task not found."));

        var controller = CreateController();

        var result = await controller.DeleteTask(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    private TasksController CreateController()
    {
        var controller = new TasksController(_taskService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
                ], "TestAuth"))
            }
        };

        return controller;
    }
}
