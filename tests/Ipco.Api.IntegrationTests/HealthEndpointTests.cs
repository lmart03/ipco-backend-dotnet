using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Ipco.Api.IntegrationTests;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public HealthEndpointTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task LiveHealth_WhenApplicationStarts_ReturnsOk()
    {
        using var response = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
