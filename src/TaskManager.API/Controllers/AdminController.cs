using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Admin.Queries;

namespace TaskManager.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
[Authorize(Policy = "AdminOnly")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users/count")]
    public async Task<ActionResult<int>> GetUserCount(CancellationToken cancellationToken)
    {
        var count = await _mediator.Send(new GetUserCountQuery(), cancellationToken);
        return Ok(count);
    }
}
