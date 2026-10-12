using Ipco.Application.Identity;
using Ipco.Application.Pagination;
using Ipco.Modules.Patients.Application;
using Ipco.Modules.Patients.Domain;

namespace Ipco.Modules.Patients.UnitTests;

public sealed class PatientServiceTests
{
    private static readonly Guid NewId = Guid.Parse("0199d000-0000-7000-8000-000000000001");
    private static readonly DateTimeOffset Now = new(2026, 10, 12, 1, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Create_WhenDocumentExists_ReturnsConflictWithoutAdding()
    {
        var repository = new FakeRepository { DocumentExists = true };
        var service = CreateService(repository);
        var data = ValidData() with { DocumentTypeId = Guid.NewGuid(), DocumentNumber = "12345678" };

        var result = await service.CreateAsync(data, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("patients.document_conflict", result.Error.Code);
        Assert.Null(repository.Added);
    }

    [Fact]
    public async Task Create_WhenValid_AddsAndSavesPatient()
    {
        var repository = new FakeRepository();
        var result = await CreateService(repository).CreateAsync(ValidData(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(NewId, result.Value);
        Assert.Equal(NewId, repository.Added?.Id);
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public async Task Get_WhenPatientDoesNotExist_ReturnsNotFound()
    {
        var result = await CreateService(new FakeRepository()).GetAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.True(result.IsFailure);
        Assert.Equal("patients.not_found", result.Error.Code);
    }

    [Fact]
    public async Task Search_WithInvalidPagination_DoesNotCallRepository()
    {
        var repository = new FakeRepository();
        var result = await CreateService(repository).SearchAsync(null, new PageRequest(0, 20), CancellationToken.None);
        Assert.True(result.IsFailure);
        Assert.Equal(0, repository.SearchCalls);
    }

    private static PatientService CreateService(FakeRepository repository) =>
        new(repository, new FixedUuidGenerator(), new FixedTimeProvider());

    private static PatientData ValidData() => new(null, null, "Ana", "Pérez", "987654321", null,
        null, null, null, null, null, null, null);

    private sealed class FixedUuidGenerator : IUuidGenerator
    {
        public Guid NewId() => PatientServiceTests.NewId;
    }

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class FakeRepository : IPatientRepository
    {
        public bool DocumentExists { get; init; }
        public Patient? Added { get; private set; }
        public int SaveCalls { get; private set; }
        public int SearchCalls { get; private set; }
        public Task<Patient?> GetAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult<Patient?>(null);
        public Task<bool> DocumentExistsAsync(Guid documentTypeId, string documentNumber, Guid? excludedId, CancellationToken cancellationToken) => Task.FromResult(DocumentExists);
        public Task<PageResult<Patient>> SearchAsync(string? search, PageRequest page, CancellationToken cancellationToken)
        {
            SearchCalls++;
            return Task.FromResult(new PageResult<Patient>([], page.Page, page.PageSize, 0));
        }
        public void Add(Patient patient) => Added = patient;
        public Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCalls++;
            return Task.FromResult(true);
        }
    }
}
