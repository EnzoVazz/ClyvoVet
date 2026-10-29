using System.Net;
using System.Net.Http.Json;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class VeterinariosControllerIntegrationTests
{
    private readonly ApiWebApplicationFactory _factory;

    public VeterinariosControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetVeterinarioById_VeterinarioNaoExistente_RetornaStatus404NotFound()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/veterinarios/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostVeterinario_NomeECrmvVazios_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);
        var veterinarioInvalido = new Veterinario { Nome = "", Crmv = "" };

        var response = await client.PostAsJsonAsync("/api/veterinarios", veterinarioInvalido);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
