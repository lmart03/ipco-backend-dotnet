using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Ipco.Web.Problems;

public static class ProblemDetailsDefaults
{
    private const string Rfc9110 = "https://www.rfc-editor.org/rfc/rfc9110.html";

    public static void Customize(ProblemDetailsContext context)
    {
        var problem = context.ProblemDetails;
        var status = problem.Status ?? context.HttpContext.Response.StatusCode;
        var defaults = ForStatus(status);
        problem.Status = status;
        problem.Type ??= defaults.Type;
        problem.Title ??= defaults.Title;
        problem.Instance ??= $"urn:uuid:{Guid.CreateVersion7()}";
        problem.Extensions.TryAdd("traceId", Activity.Current?.TraceId.ToString() ?? context.HttpContext.TraceIdentifier);
    }

    public static (string Type, string Title) ForStatus(int status) => status switch
    {
        StatusCodes.Status400BadRequest => ($"{Rfc9110}#section-15.5.1", "Bad Request"),
        StatusCodes.Status401Unauthorized => ($"{Rfc9110}#section-15.5.2", "Unauthorized"),
        StatusCodes.Status403Forbidden => ($"{Rfc9110}#section-15.5.4", "Forbidden"),
        StatusCodes.Status404NotFound => ($"{Rfc9110}#section-15.5.5", "Not Found"),
        StatusCodes.Status409Conflict => ($"{Rfc9110}#section-15.5.10", "Conflict"),
        StatusCodes.Status500InternalServerError => ($"{Rfc9110}#section-15.6.1", "Internal Server Error"),
        _ => ("about:blank", "HTTP Error")
    };
}
