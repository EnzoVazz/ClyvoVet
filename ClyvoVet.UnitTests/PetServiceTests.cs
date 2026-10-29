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
        _petService = new PetService(_petRepositoryMock.Object, AuditoriaDouble.Repository().Object, AuditoriaDouble.Usuario().Object);
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

    [Fact]
    public async Task ListarAsync_ComFiltroDeEspecie_DeveRetornarPaginaDoRepositorio()
    {
        var pagina = new ClyvoVet.Domain.Common.PagedResult<Pet>
        {
            Items = [new Pet { Nome = "Rex", Especie = "Cachorro" }],
            Page = 1,
            PageSize = 10,
            TotalCount = 1
        };

        _petRepositoryMock
            .Setup(repo => repo.ListarAsync(It.IsAny<ClyvoVet.Domain.Common.PageQuery>(), "Cachorro", null))
            .ReturnsAsync(pagina);

        var resultado = await _petService.ListarAsync(ClyvoVet.Domain.Common.PageQuery.Create(1, 10, null, null, null), "Cachorro", null);

        Assert.Equal(1, resultado.TotalCount);
        Assert.Equal("Rex", resultado.Items[0].Nome);
    }

    [Fact]
    public async Task AtualizarAsync_PetInexistente_DeveLancarNotFoundException()
    {
        _petRepositoryMock.Setup(repo => repo.ObterPorIdAsync(5)).ReturnsAsync((Pet?)null);

        var excecao = await Assert.ThrowsAsync<ClyvoVet.Domain.Exceptions.NotFoundException>(() =>
            _petService.AtualizarAsync(5, new Pet { Id = 5, Nome = "Rex", Especie = "Cachorro", IdTutor = 1 }));

        Assert.Equal("Pet não encontrado para atualização.", excecao.Message);
        _petRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Pet>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_PetComConsultas_DeveLancarConflictException()
    {
        _petRepositoryMock.Setup(repo => repo.ObterPorIdAsync(3)).ReturnsAsync(new Pet { Id = 3, Nome = "Rex" });
        _petRepositoryMock.Setup(repo => repo.PossuiConsultasAsync(3)).ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<ClyvoVet.Domain.Exceptions.ConflictException>(() => _petService.ExcluirAsync(3));

        Assert.Equal("Não é possível excluir este pet, pois ele possui consultas vinculadas no histórico.", excecao.Message);
        _petRepositoryMock.Verify(repo => repo.RemoverAsync(It.IsAny<Pet>()), Times.Never);
    }
}