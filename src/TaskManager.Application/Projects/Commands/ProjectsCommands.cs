namespace TaskManager.Application.Projects.Commands;

using Dtos;
using MediatR;

public class CreateProjectCommand : IRequest<ProjectDto>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateProjectCommand : IRequest<ProjectDto>
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class DeleteProjectCommand : IRequest<Unit>
{
    public Guid ProjectId { get; set; }
}

