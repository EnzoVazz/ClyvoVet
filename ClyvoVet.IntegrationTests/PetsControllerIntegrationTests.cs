using System.Net;
using System.Net.Http.Json;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class PetsControllerIntegrationTests
{
    private readonly ApiWebApplicationFactory _factory;

    public PetsControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetPetById_PetNaoExistente_RetornaStatus404NotFound()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/animais/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPet_NomeEEspecieVazios_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);
        var petInvalido = new Pet { IdTutor = 1, Nome = "", Especie = "" };

        var response = await client.PostAsJsonAsync("/api/animais", petInvalido);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
