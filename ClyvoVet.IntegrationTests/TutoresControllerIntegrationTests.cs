using System.Net;
using System.Net.Http.Json;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class TutoresControllerIntegrationTests
{
    private readonly ApiWebApplicationFactory _factory;

    public TutoresControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTutorById_TutorNaoExistente_RetornaStatus404NotFound()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/tutores/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostTutor_NomeECpfVazios_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);
        var tutorInvalido = new Tutor { Nome = "", Cpf = "" };

        var response = await client.PostAsJsonAsync("/api/tutores", tutorInvalido);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
