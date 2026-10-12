using Ipco.Modules.Patients.Domain;

namespace Ipco.Modules.Patients.Presentation;

/// <summary>Datos completos para registrar o reemplazar un paciente.</summary>
/// <param name="DocumentTypeId">UUID del tipo de documento; debe acompañar a documentNumber.</param>
/// <param name="DocumentNumber">Número de documento; se normaliza eliminando espacios, puntos y guiones.</param>
/// <param name="FirstName">Nombres, obligatorios, máximo 100 caracteres.</param>
/// <param name="LastName">Apellidos, obligatorios, máximo 100 caracteres.</param>
/// <param name="Phone">Teléfono obligatorio en formato internacional; se normalizan separadores.</param>
/// <param name="Email">Email opcional, normalizado a minúsculas.</param>
/// <param name="BirthDate">Fecha de nacimiento entre 1900-01-01 y hoy UTC.</param>
/// <param name="GenderId">UUID opcional del género en datos de referencia.</param>
/// <param name="MaritalStatusId">UUID opcional del estado civil en datos de referencia.</param>
/// <param name="Address">Dirección opcional, máximo 300 caracteres.</param>
/// <param name="EmergencyContactName">Nombre del contacto de emergencia.</param>
/// <param name="EmergencyContactPhone">Teléfono opcional del contacto de emergencia.</param>
/// <param name="Allergies">Alergias conocidas, máximo 2000 caracteres.</param>
public sealed record PatientRequest(
    Guid? DocumentTypeId,
    string? DocumentNumber,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    DateOnly? BirthDate,
    Guid? GenderId,
    Guid? MaritalStatusId,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Allergies)
{
    public PatientData ToData() => new(DocumentTypeId, DocumentNumber, FirstName, LastName, Phone, Email,
        BirthDate, GenderId, MaritalStatusId, Address, EmergencyContactName, EmergencyContactPhone, Allergies);
}
