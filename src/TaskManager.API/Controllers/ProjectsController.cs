using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Projects.Commands;
using TaskManager.Application.Projects.Dtos;
using TaskManager.Application.Projects.Queries;

namespace TaskManager.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Policy = "UserOrAdmin")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProjectCommand
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProjectById), new { projectId = result.Id, version = "1.0" }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAllProjects(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllProjectsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult<ProjectDto>> GetProjectById(Guid projectId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery { ProjectId = projectId }, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{projectId}")]
    public async Task<ActionResult<ProjectDto>> UpdateProject(Guid projectId, UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand
        {
            ProjectId = projectId,
            Name = request.Name,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProjectCommand { ProjectId = projectId }, cancellationToken);
        return NoContent();
    }
}
