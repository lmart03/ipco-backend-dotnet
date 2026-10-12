using FluentValidation;
using Ipco.Application.Pagination;
using Ipco.Modules.Patients.Application;
using Ipco.Web.Contracts;
using Ipco.Web.Problems;

namespace Ipco.Modules.Patients.Presentation;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientsModule(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/patients").WithTags("Patients");

        group.MapGet("/", SearchAsync).WithName("SearchPatients").WithSummary("Listar pacientes")
            .WithDescription("Devuelve pacientes activos ordenados por apellido, nombre e identificador. Busca por nombre, apellido, email, documento y teléfono.")
            .Produces<PageResult<PatientResponse>>().ProducesProblem(StatusCodes.Status400BadRequest);
        group.MapGet("/{id:guid}", GetAsync).WithName("GetPatient").WithSummary("Ver paciente")
            .Produces<PatientResponse>().ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", CreateAsync).WithName("CreatePatient").WithSummary("Registrar paciente")
            .WithDescription("Nombre, apellido y teléfono son obligatorios. Tipo y número de documento deben enviarse juntos.")
            .Accepts<PatientRequest>("application/json").Produces<CreatedResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem().ProducesProblem(StatusCodes.Status409Conflict);
        group.MapPut("/{id:guid}", UpdateAsync).WithName("UpdatePatient").WithSummary("Reemplazar paciente")
            .WithDescription("Reemplaza todos los datos editables; los campos opcionales omitidos o null quedan vacíos.")
            .Accepts<PatientRequest>("application/json").Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem().ProducesProblem(StatusCodes.Status404NotFound).ProducesProblem(StatusCodes.Status409Conflict);
        group.MapDelete("/{id:guid}", DeleteAsync).WithName("DeletePatient").WithSummary("Eliminar paciente lógicamente")
            .WithDescription("El paciente deja de aparecer en consultas y su documento queda disponible para otro registro.")
            .Produces(StatusCodes.Status204NoContent).ProducesProblem(StatusCodes.Status404NotFound);
        return endpoints;
    }

    private static async Task<IResult> SearchAsync(string? search, int page, int pageSize,
        PatientService service, CancellationToken cancellationToken)
    {
        var pagination = new PageRequest(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize);
        var result = await service.SearchAsync(search, pagination, cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> GetAsync(Guid id, PatientService service, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : result.ToProblem();
    }

    private static async Task<IResult> CreateAsync(PatientRequest request, IValidator<PatientRequest> validator,
        PatientService service, HttpContext httpContext, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ResultHttpExtensions.ValidationProblem(validation.ToDictionary());
        var result = await service.CreateAsync(request.ToData(), cancellationToken);
        if (result.IsFailure) return result.ToProblem();
        var location = $"{httpContext.Request.PathBase}/api/v1/patients/{result.Value}";
        return TypedResults.Created(location, new CreatedResponse(result.Value));
    }

    private static async Task<IResult> UpdateAsync(Guid id, PatientRequest request, IValidator<PatientRequest> validator,
        PatientService service, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid) return ResultHttpExtensions.ValidationProblem(validation.ToDictionary());
        var result = await service.UpdateAsync(id, request.ToData(), cancellationToken);
        return result.IsSuccess ? TypedResults.NoContent() : result.ToProblem();
    }

    private static async Task<IResult> DeleteAsync(Guid id, PatientService service, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? TypedResults.NoContent() : result.ToProblem();
    }
}
