namespace TaskManager.Application.Projects.Handlers;

using Commands;
using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Common;
using MediatR;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;
  private readonly ProjectCacheInvalidator _cacheInvalidator;

  public DeleteProjectCommandHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService,
      ProjectCacheInvalidator cacheInvalidator)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
  {
    var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
    if (project == null || project.UserId != _currentUserService.UserId)
    {
      throw new NotFoundException("Project", request.ProjectId);
    }

    _unitOfWork.Projects.Delete(project);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.InvalidateAsync(_currentUserService.UserId, project.Id, cancellationToken);

    return Unit.Value;
  }
}
