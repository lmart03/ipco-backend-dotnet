using Ipco.Domain;
using Microsoft.AspNetCore.Http;

namespace Ipco.Web.Problems;

public static class ResultHttpExtensions
{
    public static IResult ToProblem(this Result result)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("A successful result cannot be converted to Problem Details.");

        var status = result.Error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };
        var defaults = ProblemDetailsDefaults.ForStatus(status);
        return Results.Problem(type: defaults.Type, title: defaults.Title, statusCode: status,
            detail: result.Error.Description,
            extensions: new Dictionary<string, object?> { ["code"] = result.Error.Code });
    }

    public static IResult ValidationProblem(IDictionary<string, string[]> errors)
    {
        var defaults = ProblemDetailsDefaults.ForStatus(StatusCodes.Status400BadRequest);
        return Results.ValidationProblem(errors, type: defaults.Type, title: defaults.Title,
            extensions: new Dictionary<string, object?> { ["code"] = "request.validation_failed" });
    }
}
