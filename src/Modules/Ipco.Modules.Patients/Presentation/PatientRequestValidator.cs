using FluentValidation;
using Ipco.Modules.Patients.Domain;

namespace Ipco.Modules.Patients.Presentation;

public sealed class PatientRequestValidator : AbstractValidator<PatientRequest>
{
    public PatientRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(Patient.NameMaxLength);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(Patient.NameMaxLength);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Email).MaximumLength(Patient.EmailMaxLength).EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.DocumentNumber).MaximumLength(30);
        RuleFor(x => x.Address).MaximumLength(Patient.AddressMaxLength);
        RuleFor(x => x.EmergencyContactName).MaximumLength(Patient.EmergencyContactNameMaxLength);
        RuleFor(x => x.EmergencyContactPhone).MaximumLength(30);
        RuleFor(x => x.Allergies).MaximumLength(Patient.AllergiesMaxLength);
        RuleFor(x => x.BirthDate)
            .Must(date => date is null || date >= new DateOnly(1900, 1, 1))
            .WithMessage("birthDate no puede ser anterior a 1900-01-01.")
            .Must(date => date is null || date <= DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime))
            .WithMessage("birthDate no puede ser futura.");
        RuleFor(x => x).Must(x => x.DocumentTypeId.HasValue == !string.IsNullOrWhiteSpace(x.DocumentNumber))
            .WithName("document").WithMessage("documentTypeId y documentNumber deben enviarse juntos.");
    }
}
