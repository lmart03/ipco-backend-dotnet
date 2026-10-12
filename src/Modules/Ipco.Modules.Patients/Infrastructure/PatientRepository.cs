using Ipco.Application.Pagination;
using Ipco.Modules.Patients.Application;
using Ipco.Modules.Patients.Domain;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ipco.Modules.Patients.Infrastructure;

internal sealed class PatientRepository(PatientsDbContext context) : IPatientRepository
{
    public Task<Patient?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        context.Patients.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> DocumentExistsAsync(Guid documentTypeId, string documentNumber, Guid? excludedId, CancellationToken cancellationToken) =>
        context.Patients.AnyAsync(x => x.DocumentTypeId == documentTypeId && x.DocumentNumber == documentNumber &&
            (!excludedId.HasValue || x.Id != excludedId.Value), cancellationToken);

    public async Task<PageResult<Patient>> SearchAsync(string? search, PageRequest page, CancellationToken cancellationToken)
    {
        IQueryable<Patient> query = context.Patients.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            var pattern = $"%{EscapeLike(term)}%";
            var digits = new string(term.Where(char.IsDigit).ToArray());
            query = query.Where(x => EF.Functions.ILike(x.FirstName, pattern, "\\") ||
                EF.Functions.ILike(x.LastName, pattern, "\\") ||
                (x.Email != null && EF.Functions.ILike(x.Email, pattern, "\\")) ||
                (x.DocumentNumber != null && EF.Functions.ILike(x.DocumentNumber, pattern, "\\")) ||
                (digits.Length > 0 && x.Phone.Contains(digits)));
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.Id)
            .Skip((page.Page - 1) * page.PageSize).Take(page.PageSize).ToListAsync(cancellationToken);
        return new PageResult<Patient>(items, page.Page, page.PageSize, total);
    }

    public void Add(Patient patient) => context.Patients.Add(patient);
    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: "uk_patients_document_active"
        })
        {
            return false;
        }
    }

    private static string EscapeLike(string value) => value.Replace("\\", "\\\\", StringComparison.Ordinal)
        .Replace("%", "\\%", StringComparison.Ordinal).Replace("_", "\\_", StringComparison.Ordinal);
}
