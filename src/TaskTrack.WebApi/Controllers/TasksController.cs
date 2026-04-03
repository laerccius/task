using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;

namespace TaskTrack.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItemResponse>>> GetTasks(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var tasks = await _taskService.GetTasksAsync(userId, cancellationToken);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> GetTask(Guid id, CancellationToken cancellationToken)
    {
        var task = await _taskService.GetTaskByIdAsync(id, GetUserId(), cancellationToken);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemResponse>> CreateTask([FromBody]CreateTaskRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _taskService.CreateTaskAsync(GetUserId(), request, cancellationToken);
            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> UpdateTask(Guid id, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _taskService.UpdateTaskAsync(id, GetUserId(), request, cancellationToken);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id, GetUserId(), cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private Guid GetUserId()
    {
        var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(rawValue ?? throw new UnauthorizedAccessException("Missing user id."));
    }
}
