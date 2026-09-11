using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ClyvoVet.Domain.Entities; 

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class PetsControllerIntegrationTests
{
    private readonly HttpClient _client;

    public PetsControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPetById_PetNaoExistente_RetornaStatus404NotFound()
    {
        // Arrange
        int petIdInvalido = 99999;

        // Act
        var response = await _client.GetAsync($"/api/animais/{petIdInvalido}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostPet_NomeEEspecieVazios_RetornaStatus400BadRequest()
    {
        // Arrange
        var petInvalido = new Pet
        {
            IdTutor = 1,
            Nome = "", 
            Especie = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/animais", petInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}