using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;

namespace ClyvoVet.UnitTests;

public class ConsultaServiceTests
{
    private readonly Mock<IConsultaRepository> _consultaRepositoryMock;
    private readonly ConsultaService _consultaService;

    public ConsultaServiceTests()
    {
        _consultaRepositoryMock = new Mock<IConsultaRepository>();
        _consultaService = new ConsultaService(_consultaRepositoryMock.Object);
    }

    [Fact]
    public async Task AgendarConsultaAsync_DadosValidos_DeveDefinirStatusSalvarConsulta()
    {
        var novaConsulta = new Consulta 
        { 
            IdVeterinario = 1, 
            IdPet = 10, 
            IdClinica = 5,
            DataConsulta = DateTime.Now.AddDays(2) 
        };
        
        _consultaRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Consulta>())).ReturnsAsync(novaConsulta);

        var resultado = await _consultaService.AgendarConsultaAsync(novaConsulta);

        Assert.NotNull(resultado);
        Assert.Equal("Agendada", resultado.Status);
        
        _consultaRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Consulta>()), Times.Once);
    }

    [Theory]
    [InlineData(0, 10, 5)]
    [InlineData(1, 0, 5)]
    [InlineData(1, 10, 0)]
    [InlineData(-1, -1, -1)]
    public async Task AgendarConsultaAsync_IdsInvalidos_DeveLancarArgumentException(int idVet, int idPet, int idClinica)
    {
        var consultaInvalida = new Consulta 
        { 
            IdVeterinario = idVet, 
            IdPet = idPet, 
            IdClinica = idClinica 
        };

        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _consultaService.AgendarConsultaAsync(consultaInvalida));
        
        Assert.Equal("Os IDs do Veterinário, Pet e Clínica são obrigatórios para agendar uma consulta.", excecao.Message);
        
        _consultaRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Consulta>()), Times.Never);
    }
}
