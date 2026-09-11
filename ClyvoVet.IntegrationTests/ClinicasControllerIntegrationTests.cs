using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ClyvoVet.Domain.Entities; 

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class ClinicasControllerIntegrationTests
{
    private readonly HttpClient _client;

    public ClinicasControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetClinicaById_ClinicaNaoExistente_RetornaStatus404NotFound()
    {
        // Arrange
        int clinicaIdInvalida = 99999;

        // Act
        var response = await _client.GetAsync($"/api/clinicas/{clinicaIdInvalida}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostClinica_NomeECnpjVazios_RetornaStatus400BadRequest()
    {
        // Arrange
        var clinicaInvalida = new Clinica
        {
            Nome = "", 
            Cnpj = "", 
            Logradouro = "Rua Fictícia, 123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clinicas", clinicaInvalida);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}