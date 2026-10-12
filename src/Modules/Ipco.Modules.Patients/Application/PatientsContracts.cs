using Ipco.Application.Pagination;
using Ipco.Modules.Patients.Domain;

namespace Ipco.Modules.Patients.Application;

public interface IPatientRepository
{
    Task<Patient?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> DocumentExistsAsync(Guid documentTypeId, string documentNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task<PageResult<Patient>> SearchAsync(string? search, PageRequest page, CancellationToken cancellationToken);
    void Add(Patient patient);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}
public sealed record PatientResponse(
    Guid Id,
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
    string? Allergies,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc)
{
    public static PatientResponse From(Patient patient) => new(
        patient.Id, patient.DocumentTypeId, patient.DocumentNumber, patient.FirstName, patient.LastName,
        patient.Phone, patient.Email, patient.BirthDate, patient.GenderId, patient.MaritalStatusId,
        patient.Address, patient.EmergencyContactName, patient.EmergencyContactPhone, patient.Allergies,
        patient.CreatedAtUtc, patient.UpdatedAtUtc);
}
