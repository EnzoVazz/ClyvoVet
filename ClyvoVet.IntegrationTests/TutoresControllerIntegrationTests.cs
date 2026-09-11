using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ClyvoVet.Domain.Entities; 

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class TutoresControllerIntegrationTests
{
    private readonly HttpClient _client;

    public TutoresControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTutorById_TutorNaoExistente_RetornaStatus404NotFound()
    {
        // Arrange
        int tutorIdInvalido = 99999;

        // Act
        var response = await _client.GetAsync($"/api/tutores/{tutorIdInvalido}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostTutor_NomeECpfVazios_RetornaStatus400BadRequest()
    {
        // Arrange
        var tutorInvalido = new Tutor
        {
            Nome = "", // Forçando a validação de string.IsNullOrWhiteSpace
            Cpf = ""   // Forçando a validação de string.IsNullOrWhiteSpace
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tutores", tutorInvalido);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}