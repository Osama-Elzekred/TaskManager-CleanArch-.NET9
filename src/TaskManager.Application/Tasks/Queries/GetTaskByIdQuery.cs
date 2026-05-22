namespace TaskManager.Application.Tasks.Queries;

using Dtos;
using MediatR;

public class GetTaskByIdQuery : IRequest<TaskDto>
{
  public Guid TaskId { get; set; }
  public Guid ProjectId { get; set; }
}
