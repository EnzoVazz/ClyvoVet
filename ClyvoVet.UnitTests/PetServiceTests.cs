using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;

namespace ClyvoVet.UnitTests;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _petRepositoryMock;
    private readonly PetService _petService;

    public PetServiceTests()
    {
        _petRepositoryMock = new Mock<IPetRepository>();
        _petService = new PetService(_petRepositoryMock.Object);
    }

    [Fact]
    public async Task CadastrarPetAsync_DadosValidos_DeveRetornarPetSalvo()
    {
        var novoPet = new Pet { Nome = "Rex", Especie = "Cachorro", IdTutor = 1 };
        
        _petRepositoryMock
            .Setup(repo => repo.AdicionarAsync(It.IsAny<Pet>()))
            .ReturnsAsync(novoPet);

        var resultado = await _petService.CadastrarPetAsync(novoPet);

        Assert.NotNull(resultado);
        Assert.Equal("Rex", resultado.Nome);
        
        _petRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Pet>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarPetAsync_NomeVazio_DeveLancarExcecao()
    {
        var petInvalido = new Pet { Nome = "", Especie = "Gato", IdTutor = 1 };

        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _petService.CadastrarPetAsync(petInvalido));
        
        Assert.Equal("O Nome e a Espécie do Pet são obrigatórios.", excecao.Message);
        
        _petRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Pet>()), Times.Never);
    }
}