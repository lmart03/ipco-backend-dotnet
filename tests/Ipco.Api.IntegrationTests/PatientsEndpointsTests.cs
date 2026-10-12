using System.Net;
using System.Net.Http.Json;
using Ipco.Application.Pagination;
using Ipco.Modules.Patients.Application;
using Ipco.Web.Contracts;

namespace Ipco.Api.IntegrationTests;

public sealed class PatientsEndpointsTests(PatientsApiFactory factory) : IClassFixture<PatientsApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateGetSearchUpdateDelete_HappyPath()
    {
        var request = ValidRequest();
        using var create = await _client.PostAsJsonAsync("/api/v1/patients", request);
        var created = await create.Content.ReadFromJsonAsync<CreatedResponse>();

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        Assert.NotNull(created);
        Assert.Equal($"/api/v1/patients/{created.Id}", create.Headers.Location?.ToString());

        var patient = await _client.GetFromJsonAsync<PatientResponse>($"/api/v1/patients/{created.Id}");
        Assert.NotNull(patient);
        Assert.Equal("Ana", patient.FirstName);
        Assert.Equal("+51987654321", patient.Phone);
        Assert.Equal("ana@mail.com", patient.Email);

        var page = await _client.GetFromJsonAsync<PageResult<PatientResponse>>("/api/v1/patients?search=987-654&page=1&pageSize=20");
        Assert.NotNull(page);
        Assert.Contains(page.Items, x => x.Id == created.Id);

        using var update = await _client.PutAsJsonAsync($"/api/v1/patients/{created.Id}", request with
        {
            FirstName = "María",
            Email = "maria@mail.com"
        });
        Assert.Equal(HttpStatusCode.NoContent, update.StatusCode);
        Assert.Empty(await update.Content.ReadAsByteArrayAsync());

        var updated = await _client.GetFromJsonAsync<PatientResponse>($"/api/v1/patients/{created.Id}");
        Assert.NotNull(updated);
        Assert.Equal("María", updated.FirstName);
        Assert.Equal("maria@mail.com", updated.Email);

        using var delete = await _client.DeleteAsync($"/api/v1/patients/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Empty(await delete.Content.ReadAsByteArrayAsync());
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync($"/api/v1/patients/{created.Id}")).StatusCode);
    }

    [Fact]
    public async Task Create_WithDuplicateDocument_ReturnsConflictProblemDetails()
    {
        var documentTypeId = Guid.CreateVersion7();
        var request = ValidRequest() with { DocumentTypeId = documentTypeId, DocumentNumber = "12345678" };
        Assert.Equal(HttpStatusCode.Created, (await _client.PostAsJsonAsync("/api/v1/patients", request)).StatusCode);

        using var duplicate = await _client.PostAsJsonAsync("/api/v1/patients", request);
        var body = await duplicate.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
        Assert.Equal("application/problem+json", duplicate.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Contains("traceId", body.Keys);
    }

    [Fact]
    public async Task Create_WithInvalidRequest_ReturnsValidationProblem()
    {
        using var response = await _client.PostAsJsonAsync("/api/v1/patients", ValidRequest() with { FirstName = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    private static PatientApiRequest ValidRequest() => new(null, null, " Ana ", "Pérez", "+51 987-654-321",
        "ANA@MAIL.COM", null, null, null, null, null, null, null);

    private sealed record PatientApiRequest(Guid? DocumentTypeId, string? DocumentNumber, string? FirstName,
        string? LastName, string? Phone, string? Email, DateOnly? BirthDate, Guid? GenderId,
        Guid? MaritalStatusId, string? Address, string? EmergencyContactName, string? EmergencyContactPhone,
        string? Allergies);
}
