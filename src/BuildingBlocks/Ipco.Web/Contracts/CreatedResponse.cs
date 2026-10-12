namespace Ipco.Web.Contracts;

/// <summary>Respuesta estándar de toda operación POST que crea un recurso.</summary>
/// <param name="Id">UUID del recurso creado.</param>
public sealed record CreatedResponse(Guid Id);
