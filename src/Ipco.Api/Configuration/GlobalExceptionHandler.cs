using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ipco.Api.Configuration;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception while processing {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred.",
                Extensions = { ["code"] = "server.unexpected_error" }
            }
        });
    }
}
