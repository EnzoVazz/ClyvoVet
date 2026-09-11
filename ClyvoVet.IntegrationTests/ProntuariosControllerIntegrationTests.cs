using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ClyvoVet.Domain.Entities; 

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class ProntuariosControllerIntegrationTests
{
    private readonly HttpClient _client;

    public ProntuariosControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProntuarioById_ProntuarioNaoExistente_RetornaStatus404NotFound()
    {
        // Arrange
        int prontuarioIdInvalido = 99999;

        // Act
        var response = await _client.GetAsync($"/api/prontuarios/{prontuarioIdInvalido}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostProntuario_DiagnosticoVazio_RetornaStatus400BadRequest()
    {
        // Arrange
        var prontuarioInvalido = new Prontuario
        {
            Diagnostico = "", 
            DataRegistro = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/prontuarios", prontuarioInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}