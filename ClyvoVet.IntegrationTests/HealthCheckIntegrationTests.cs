using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class HealthCheckIntegrationTests
{
    private readonly HttpClient _client;

    public HealthCheckIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthCheck_EndpointConfigurado_RetornaStatus200OK()
    {
        var response = await _client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}