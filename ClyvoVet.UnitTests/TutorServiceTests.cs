using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;

namespace ClyvoVet.UnitTests;

public class TutorServiceTests
{
    private readonly Mock<ITutorRepository> _tutorRepositoryMock;
    private readonly TutorService _tutorService;

    public TutorServiceTests()
    {
        _tutorRepositoryMock = new Mock<ITutorRepository>();
        _tutorService = new TutorService(_tutorRepositoryMock.Object, AuditoriaDouble.Repository().Object, AuditoriaDouble.Usuario().Object);
    }

    [Fact]
    public async Task CadastrarTutorAsync_DadosValidosECpfInedito_DeveRetornarTutorSalvo()
    {
        var novoTutor = new Tutor { Nome = "Enzo Vaz", Cpf = "12345678900" };
        
        // Simula que o CPF ainda não existe no banco
        _tutorRepositoryMock.Setup(repo => repo.ExisteCpfAsync(novoTutor.Cpf)).ReturnsAsync(false);
        // Simula o salvamento
        _tutorRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Tutor>())).ReturnsAsync(novoTutor);

        var resultado = await _tutorService.CadastrarTutorAsync(novoTutor);

        Assert.NotNull(resultado);
        Assert.Equal("Enzo Vaz", resultado.Nome);
        
        _tutorRepositoryMock.Verify(repo => repo.ExisteCpfAsync(novoTutor.Cpf), Times.Once);
        _tutorRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Tutor>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarTutorAsync_CpfVazio_DeveLancarArgumentException()
    {
        var tutorInvalido = new Tutor { Nome = "João", Cpf = "" };

        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _tutorService.CadastrarTutorAsync(tutorInvalido));
        
        Assert.Equal("O Nome e o CPF do Tutor são obrigatórios.", excecao.Message);
        
        _tutorRepositoryMock.Verify(repo => repo.ExisteCpfAsync(It.IsAny<string>()), Times.Never);
        _tutorRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarTutorAsync_CpfJaExistente_DeveLancarInvalidOperationException()
    {
        var tutorDuplicado = new Tutor { Nome = "Maria", Cpf = "98765432100" };
        
        // Força o Mock a devolver true, simulando que a constraint do banco apitou
        _tutorRepositoryMock.Setup(repo => repo.ExisteCpfAsync(tutorDuplicado.Cpf)).ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _tutorService.CadastrarTutorAsync(tutorDuplicado));
        
        Assert.Equal("Já existe um tutor cadastrado com este CPF.", excecao.Message);
        
        // Garante que tentou consultar o CPF, mas foi bloqueado de salvar
        _tutorRepositoryMock.Verify(repo => repo.ExisteCpfAsync(tutorDuplicado.Cpf), Times.Once);
        _tutorRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_CpfDeOutroTutor_DeveLancarInvalidOperationException()
    {
        var dados = new Tutor { Id = 2, Nome = "Maria", Cpf = "98765432100" };
        _tutorRepositoryMock.Setup(repo => repo.ObterPorIdAsync(2)).ReturnsAsync(new Tutor { Id = 2, Nome = "Maria", Cpf = "111" });
        _tutorRepositoryMock.Setup(repo => repo.ObterPorCpfAsync(dados.Cpf)).ReturnsAsync(new Tutor { Id = 9, Cpf = dados.Cpf });

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _tutorService.AtualizarAsync(2, dados));

        Assert.Equal("Já existe um tutor cadastrado com este CPF.", excecao.Message);
        _tutorRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Tutor>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_TutorComPets_DeveLancarConflictException()
    {
        _tutorRepositoryMock.Setup(repo => repo.ObterPorIdAsync(4)).ReturnsAsync(new Tutor { Id = 4, Nome = "Ana" });
        _tutorRepositoryMock.Setup(repo => repo.PossuiPetsAsync(4)).ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<ClyvoVet.Domain.Exceptions.ConflictException>(() => _tutorService.ExcluirAsync(4));

        Assert.Equal("Não é possível excluir este tutor, pois ele possui pets vinculados.", excecao.Message);
        _tutorRepositoryMock.Verify(repo => repo.RemoverAsync(It.IsAny<Tutor>()), Times.Never);
    }
}