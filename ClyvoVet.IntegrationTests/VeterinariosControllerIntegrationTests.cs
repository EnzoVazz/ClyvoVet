using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ClyvoVet.Domain.Entities; 

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class VeterinariosControllerIntegrationTests
{
    private readonly HttpClient _client;

    public VeterinariosControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetVeterinarioById_VeterinarioNaoExistente_RetornaStatus404NotFound()
    {
        // Arrange
        int vetIdInvalido = 99999;

        // Act
        var response = await _client.GetAsync($"/api/veterinarios/{vetIdInvalido}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostVeterinario_NomeECrmvVazios_RetornaStatus400BadRequest()
    {
        // Arrange
        var veterinarioInvalido = new Veterinario
        {
            Nome = "", 
            Crmv = ""  
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/veterinarios", veterinarioInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}