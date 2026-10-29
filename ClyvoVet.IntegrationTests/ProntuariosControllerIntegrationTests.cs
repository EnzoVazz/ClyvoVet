using System.Net;
using System.Net.Http.Json;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class ProntuariosControllerIntegrationTests
{
    private readonly ApiWebApplicationFactory _factory;

    public ProntuariosControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProntuarioById_ProntuarioNaoExistente_RetornaStatus404NotFound()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/prontuarios/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostProntuario_DiagnosticoVazio_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);
        var prontuarioInvalido = new Prontuario { Diagnostico = "" };

        var response = await client.PostAsJsonAsync("/api/prontuarios", prontuarioInvalido);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
