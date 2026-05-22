using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.API.Filters;

/// <summary>
/// Wraps successful payloads in <see cref="ApiResponse{T}"/> (bonus: generic response wrapper).
/// Errors use the same shape via <see cref="IExceptionHandler"/>.
/// </summary>
public sealed class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: not null } objectResult
            && objectResult.StatusCode is not StatusCodes.Status204NoContent
            && !IsAlreadyWrapped(objectResult.Value))
        {
            objectResult.Value = ApiResponse<object>.SuccessResponse(objectResult.Value);
        }

        var cacheContext = context.HttpContext.RequestServices.GetService<ICacheRequestContext>();
        if (cacheContext?.HasCacheStatus == true)
        {
            context.HttpContext.Response.Headers["X-Cache-Status"] = cacheContext.WasCacheHit ? "HIT" : "MISS";
        }

        await next();
    }

    private static bool IsAlreadyWrapped(object value) =>
        value.GetType().IsGenericType
        && value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>);
}
