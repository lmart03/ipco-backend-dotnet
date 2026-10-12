using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace Ipco.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class ProblemDetailsTests
{
    private readonly HttpClient _client;

    public ProblemDetailsTests(PatientsApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task UnknownRoute_ReturnsRfc9457ProblemDetails()
    {
        using var response = await _client.GetAsync("/route-that-does-not-exist");
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
        Assert.NotNull(problem.Type);
        Assert.StartsWith("urn:uuid:", problem.Instance, StringComparison.Ordinal);
        Assert.True(problem.Extensions.ContainsKey("traceId"));
    }
}
