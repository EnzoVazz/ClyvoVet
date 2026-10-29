using System.Net;
using System.Net.Http.Json;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class ConsultasControllerIntegrationTests
{
    private readonly ApiWebApplicationFactory _factory;

    public ConsultasControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetConsultaById_ConsultaNaoExistente_RetornaStatus404NotFound()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/consultas/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostConsulta_IdsZerados_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);
        var consultaInvalida = new Consulta { IdVeterinario = 0, IdPet = 0, IdClinica = 0 };

        var response = await client.PostAsJsonAsync("/api/consultas", consultaInvalida);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
