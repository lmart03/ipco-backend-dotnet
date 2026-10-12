using Ipco.Domain;

namespace Ipco.Modules.Patients.Domain;

public static class PatientErrors
{
    public static readonly Error NotFound = Error.NotFound("patients.not_found", "Paciente no encontrado.");
    public static readonly Error DuplicateDocument = Error.Conflict("patients.document_conflict", "Ya existe un paciente con ese documento.");

    public static Error Invalid(string field, string description) =>
        Error.Validation($"patients.{field}_invalid", description);
}
