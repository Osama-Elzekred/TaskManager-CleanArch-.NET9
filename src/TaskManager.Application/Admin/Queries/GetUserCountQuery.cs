using MediatR;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Application.Admin.Queries;

public record GetUserCountQuery : IRequest<int>;

public class GetUserCountQueryHandler : IRequestHandler<GetUserCountQuery, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserCountQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(GetUserCountQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        return users.Count();
    }
}
