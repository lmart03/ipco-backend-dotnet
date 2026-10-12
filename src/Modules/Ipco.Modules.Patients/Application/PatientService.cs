using Ipco.Application.Identity;
using Ipco.Application.Pagination;
using Ipco.Domain;
using Ipco.Modules.Patients.Domain;

namespace Ipco.Modules.Patients.Application;

public sealed class PatientService(IPatientRepository repository, IUuidGenerator uuidGenerator, TimeProvider timeProvider)
{
    public async Task<Result<PatientResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await repository.GetAsync(id, cancellationToken);
        return patient is null
            ? Result<PatientResponse>.Failure(PatientErrors.NotFound)
            : Result<PatientResponse>.Success(PatientResponse.From(patient));
    }

    public async Task<Result<PageResult<PatientResponse>>> SearchAsync(string? search, PageRequest page, CancellationToken cancellationToken)
    {
        if (page.Page < 1)
            return Result<PageResult<PatientResponse>>.Failure(PatientErrors.Invalid("page", "page debe ser mayor o igual a 1."));
        if (page.PageSize is < 1 or > PageRequest.MaximumPageSize)
            return Result<PageResult<PatientResponse>>.Failure(PatientErrors.Invalid("page_size", "pageSize debe estar entre 1 y 100."));

        var result = await repository.SearchAsync(search, page, cancellationToken);
        return Result<PageResult<PatientResponse>>.Success(new PageResult<PatientResponse>(
            result.Items.Select(PatientResponse.From).ToArray(), result.Page, result.PageSize, result.TotalItems));
    }

    public async Task<Result<Guid>> CreateAsync(PatientData data, CancellationToken cancellationToken)
    {
        var created = Patient.Create(uuidGenerator.NewId(), data, timeProvider.GetUtcNow());
        if (created.IsFailure) return Result<Guid>.Failure(created.Error);
        if (await HasDuplicateDocument(created.Value, excludedId: null, cancellationToken))
            return Result<Guid>.Failure(PatientErrors.DuplicateDocument);

        repository.Add(created.Value);
        if (!await repository.SaveChangesAsync(cancellationToken))
            return Result<Guid>.Failure(PatientErrors.DuplicateDocument);
        return Result<Guid>.Success(created.Value.Id);
    }

    public async Task<Result> UpdateAsync(Guid id, PatientData data, CancellationToken cancellationToken)
    {
        var patient = await repository.GetAsync(id, cancellationToken);
        if (patient is null) return Result.Failure(PatientErrors.NotFound);

        var updated = patient.Update(data, timeProvider.GetUtcNow());
        if (updated.IsFailure) return updated;
        if (await HasDuplicateDocument(patient, id, cancellationToken))
            return Result.Failure(PatientErrors.DuplicateDocument);

        if (!await repository.SaveChangesAsync(cancellationToken))
            return Result.Failure(PatientErrors.DuplicateDocument);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await repository.GetAsync(id, cancellationToken);
        if (patient is null) return Result.Failure(PatientErrors.NotFound);
        patient.Delete(timeProvider.GetUtcNow());
        await repository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private Task<bool> HasDuplicateDocument(Patient patient, Guid? excludedId, CancellationToken cancellationToken) =>
        patient.DocumentTypeId is { } typeId && patient.DocumentNumber is { } number
            ? repository.DocumentExistsAsync(typeId, number, excludedId, cancellationToken)
            : Task.FromResult(false);
}
