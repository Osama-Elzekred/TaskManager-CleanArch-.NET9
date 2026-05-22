using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks.Dtos;
using TaskManager.Application.Tasks.Queries;
using TaskItemStatus = TaskManager.Application.Tasks.Dtos.TaskStatus;

namespace TaskManager.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/projects/{projectId}/tasks")]
[Authorize(Policy = "UserOrAdmin")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> CreateTask(Guid projectId, CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand
        {
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Status = (TaskItemStatus)request.Status,
            DueDate = request.DueDate,
            Priority = (Priority)request.Priority
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetTaskById), new { projectId, taskId = result.Id, version = "1.0" }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetTasksByProject(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery { ProjectId = projectId }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult<TaskDto>> GetTaskById(Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery { ProjectId = projectId, TaskId = taskId }, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{taskId}/status")]
    public async Task<ActionResult<TaskDto>> UpdateTaskStatus(
        Guid projectId,
        Guid taskId,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTaskStatusCommand
        {
            ProjectId = projectId,
            TaskId = taskId,
            Status = request.Status
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{taskId}")]
    public async Task<ActionResult<TaskDto>> UpdateTask(Guid projectId, Guid taskId, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTaskCommand
        {
            TaskId = taskId,
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate,
            Priority = request.Priority
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(Guid projectId, Guid taskId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTaskCommand { TaskId = taskId, ProjectId = projectId }, cancellationToken);
        return NoContent();
    }
}
