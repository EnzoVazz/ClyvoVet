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
        _tutorService = new TutorService(_tutorRepositoryMock.Object);
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
}