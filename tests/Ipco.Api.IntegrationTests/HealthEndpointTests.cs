using System.Net;

namespace Ipco.Api.IntegrationTests;

[Collection(ApiCollection.Name)]
public sealed class HealthEndpointTests
{
    private readonly HttpClient _client;
    public HealthEndpointTests(PatientsApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task LiveHealth_WhenApplicationStarts_ReturnsOk()
    {
        using var response = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
