using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;

namespace ClyvoVet.UnitTests;

public class VeterinarioServiceTests
{
    private readonly Mock<IVeterinarioRepository> _veterinarioRepositoryMock;
    private readonly VeterinarioService _veterinarioService;

    public VeterinarioServiceTests()
    {
        _veterinarioRepositoryMock = new Mock<IVeterinarioRepository>();
        _veterinarioService = new VeterinarioService(_veterinarioRepositoryMock.Object);
    }

    [Fact]
    public async Task CadastrarVeterinarioAsync_DadosValidosECrmvInedito_DeveRetornarVeterinarioSalvo()
    {
        var novoVeterinario = new Veterinario { Nome = "Dra. Letícia", Crmv = "12345-SP" };
        
        _veterinarioRepositoryMock.Setup(repo => repo.ExisteCrmvAsync(novoVeterinario.Crmv)).ReturnsAsync(false);
        _veterinarioRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Veterinario>())).ReturnsAsync(novoVeterinario);

        var resultado = await _veterinarioService.CadastrarVeterinarioAsync(novoVeterinario);

        Assert.NotNull(resultado);
        Assert.Equal("Dra. Letícia", resultado.Nome);
        
        _veterinarioRepositoryMock.Verify(repo => repo.ExisteCrmvAsync(novoVeterinario.Crmv), Times.Once);
        _veterinarioRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Veterinario>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarVeterinarioAsync_CrmvVazio_DeveLancarArgumentException()
    {
        var veterinarioInvalido = new Veterinario { Nome = "Dr. Carlos", Crmv = "" };

        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _veterinarioService.CadastrarVeterinarioAsync(veterinarioInvalido));
        
        Assert.Equal("O Nome e o CRMV do Veterinário são obrigatórios.", excecao.Message);
        
        _veterinarioRepositoryMock.Verify(repo => repo.ExisteCrmvAsync(It.IsAny<string>()), Times.Never);
        _veterinarioRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Veterinario>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarVeterinarioAsync_CrmvJaExistente_DeveLancarInvalidOperationException()
    {
        var veterinarioDuplicado = new Veterinario { Nome = "Dr. Roberto", Crmv = "99887-RJ" };
        
        _veterinarioRepositoryMock.Setup(repo => repo.ExisteCrmvAsync(veterinarioDuplicado.Crmv)).ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _veterinarioService.CadastrarVeterinarioAsync(veterinarioDuplicado));
        
        Assert.Equal("Já existe um veterinário cadastrado com este CRMV.", excecao.Message);
        
        _veterinarioRepositoryMock.Verify(repo => repo.ExisteCrmvAsync(veterinarioDuplicado.Crmv), Times.Once);
        _veterinarioRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Veterinario>()), Times.Never);
    }
}
