using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ClyvoVet.Domain.Entities; 

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class ConsultasControllerIntegrationTests
{
    private readonly HttpClient _client;

    public ConsultasControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetConsultaById_ConsultaNaoExistente_RetornaStatus404NotFound()
    {
        // Arrange
        int consultaIdInvalida = 99999;

        // Act
        var response = await _client.GetAsync($"/api/consultas/{consultaIdInvalida}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostConsulta_IdsZerados_RetornaStatus400BadRequest()
    {
        // Arrange
        var consultaInvalida = new Consulta
        {
            IdVeterinario = 0, 
            IdPet = 0,
            IdClinica = 0,
            DataConsulta = DateTime.Now.AddDays(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/consultas", consultaInvalida);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}