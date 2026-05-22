namespace TaskManager.Application.Projects.Queries;

using Dtos;
using MediatR;

public class GetTasksByProjectQuery : IRequest<List<TaskDto>>
{
  public Guid ProjectId { get; set; }
}
