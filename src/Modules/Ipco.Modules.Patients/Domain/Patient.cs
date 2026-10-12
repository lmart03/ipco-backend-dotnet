using System.Text.RegularExpressions;
using Ipco.Domain;

namespace Ipco.Modules.Patients.Domain;

public sealed partial class Patient : AggregateRoot
{
    public const int NameMaxLength = 100;
    public const int AddressMaxLength = 300;
    public const int EmergencyContactNameMaxLength = 150;
    public const int AllergiesMaxLength = 2000;
    public const int EmailMaxLength = 254;
    public const int DocumentNumberMaxLength = 20;

    private Patient() { }

    private Patient(Guid id, DateTimeOffset createdAtUtc) : base(id) => CreatedAtUtc = createdAtUtc;

    public Guid? DocumentTypeId { get; private set; }
    public string? DocumentNumber { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? Email { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public Guid? GenderId { get; private set; }
    public Guid? MaritalStatusId { get; private set; }
    public string? Address { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public string? Allergies { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static Result<Patient> Create(Guid id, PatientData data, DateTimeOffset now)
    {
        var patient = new Patient(id, now);
        var result = patient.Apply(data, now, isCreation: true);
        return result.IsFailure ? Result<Patient>.Failure(result.Error) : Result<Patient>.Success(patient);
    }

    public Result Update(PatientData data, DateTimeOffset now) => Apply(data, now, isCreation: false);

    public void Delete(DateTimeOffset now)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedAtUtc = now;
        UpdatedAtUtc = now;
    }

    private Result Apply(PatientData data, DateTimeOffset now, bool isCreation)
    {
        var firstName = Required(data.FirstName, NameMaxLength, "first_name", "firstName es obligatorio y admite hasta 100 caracteres");
        if (firstName.IsFailure) return firstName;
        var lastName = Required(data.LastName, NameMaxLength, "last_name", "lastName es obligatorio y admite hasta 100 caracteres");
        if (lastName.IsFailure) return lastName;
        var phone = NormalizePhone(data.Phone, "phone");
        if (phone.IsFailure) return phone;
        var email = NormalizeEmail(data.Email);
        if (email.IsFailure) return email;
        var document = NormalizeDocument(data.DocumentTypeId, data.DocumentNumber);
        if (document.IsFailure) return document;
        if (data.BirthDate is { } date && (date < new DateOnly(1900, 1, 1) || date > DateOnly.FromDateTime(now.UtcDateTime)))
            return Result.Failure(PatientErrors.Invalid("birth_date", "La fecha de nacimiento no puede ser futura ni anterior a 1900."));

        var address = Optional(data.Address, AddressMaxLength, "address");
        if (address.IsFailure) return address;
        var emergencyName = Optional(data.EmergencyContactName, EmergencyContactNameMaxLength, "emergency_contact_name");
        if (emergencyName.IsFailure) return emergencyName;
        var emergencyPhone = string.IsNullOrWhiteSpace(data.EmergencyContactPhone)
            ? Result<string?>.Success(null)
            : NormalizePhone(data.EmergencyContactPhone, "emergency_contact_phone").Map<string?>(value => value);
        if (emergencyPhone.IsFailure) return emergencyPhone;
        var allergies = Optional(data.Allergies, AllergiesMaxLength, "allergies");
        if (allergies.IsFailure) return allergies;

        FirstName = firstName.Value;
        LastName = lastName.Value;
        Phone = phone.Value;
        Email = email.Value;
        DocumentTypeId = data.DocumentTypeId;
        DocumentNumber = document.Value;
        BirthDate = data.BirthDate;
        GenderId = data.GenderId;
        MaritalStatusId = data.MaritalStatusId;
        Address = address.Value;
        EmergencyContactName = emergencyName.Value;
        EmergencyContactPhone = emergencyPhone.Value;
        Allergies = allergies.Value;
        if (!isCreation) UpdatedAtUtc = now;
        return Result.Success();
    }

    private static Result<string> Required(string? value, int max, string field, string message)
    {
        var normalized = value?.Trim();
        return string.IsNullOrEmpty(normalized) || normalized.Length > max
            ? Result<string>.Failure(PatientErrors.Invalid(field, message))
            : Result<string>.Success(normalized);
    }

    private static Result<string?> Optional(string? value, int max, string field)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result<string?>.Success(null);
        var normalized = value.Trim();
        return normalized.Length > max
            ? Result<string?>.Failure(PatientErrors.Invalid(field, $"{field} admite hasta {max} caracteres."))
            : Result<string?>.Success(normalized);
    }

    private static Result<string?> NormalizeEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result<string?>.Success(null);
        var normalized = value.Trim().ToLowerInvariant();
        return normalized.Length <= EmailMaxLength && EmailRegex().IsMatch(normalized)
            ? Result<string?>.Success(normalized)
            : Result<string?>.Failure(PatientErrors.Invalid("email", "El email no tiene un formato válido."));
    }

    private static Result<string> NormalizePhone(string? value, string field)
    {
        var normalized = value is null ? "" : PhoneSeparatorsRegex().Replace(value.Trim(), "");
        return PhoneRegex().IsMatch(normalized)
            ? Result<string>.Success(normalized)
            : Result<string>.Failure(PatientErrors.Invalid(field, "El teléfono debe tener entre 6 y 15 dígitos, con '+' opcional al inicio."));
    }

    private static Result<string?> NormalizeDocument(Guid? typeId, string? number)
    {
        var hasNumber = !string.IsNullOrWhiteSpace(number);
        if (typeId is null && !hasNumber) return Result<string?>.Success(null);
        if (typeId is null || !hasNumber)
            return Result<string?>.Failure(PatientErrors.Invalid("document", "documentTypeId y documentNumber deben enviarse juntos."));

        var normalized = DocumentSeparatorsRegex().Replace(number!.Trim(), "").ToUpperInvariant();
        return DocumentRegex().IsMatch(normalized)
            ? Result<string?>.Success(normalized)
            : Result<string?>.Failure(PatientErrors.Invalid("document_number", "El número de documento admite hasta 20 letras o dígitos."));
    }

    [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")]
    private static partial Regex EmailRegex();
    [GeneratedRegex("[\\s\\-.()]")]
    private static partial Regex PhoneSeparatorsRegex();
    [GeneratedRegex("^\\+?[0-9]{6,15}$")]
    private static partial Regex PhoneRegex();
    [GeneratedRegex("[\\s\\-.]")]
    private static partial Regex DocumentSeparatorsRegex();
    [GeneratedRegex("^[A-Z0-9]{1,20}$")]
    private static partial Regex DocumentRegex();
}

public sealed record PatientData(
    Guid? DocumentTypeId,
    string? DocumentNumber,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Email,
    DateOnly? BirthDate,
    Guid? GenderId,
    Guid? MaritalStatusId,
    string? Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? Allergies);
