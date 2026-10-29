using System.Net;
using System.Net.Http.Json;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class ClinicasControllerIntegrationTests
{
    private readonly ApiWebApplicationFactory _factory;

    public ClinicasControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetClinicaById_ClinicaNaoExistente_RetornaStatus404NotFound()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/clinicas/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostClinica_NomeECnpjVazios_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);
        var clinicaInvalida = new Clinica { Nome = "", Cnpj = "" };

        var response = await client.PostAsJsonAsync("/api/clinicas", clinicaInvalida);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
