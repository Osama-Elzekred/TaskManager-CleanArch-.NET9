namespace TaskManager.Application.Tasks.Commands;

using MediatR;
using Dtos;

public class UpdateTaskStatusCommand : IRequest<TaskDto>
{
  public Guid TaskId { get; set; }
  public Guid ProjectId { get; set; }
  public TaskStatus Status { get; set; }
}
