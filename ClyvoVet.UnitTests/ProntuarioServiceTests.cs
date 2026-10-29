using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;

namespace ClyvoVet.UnitTests;

public class ProntuarioServiceTests
{
    private readonly Mock<IProntuarioRepository> _prontuarioRepositoryMock;
    private readonly ProntuarioService _prontuarioService;

    public ProntuarioServiceTests()
    {
        _prontuarioRepositoryMock = new Mock<IProntuarioRepository>();
        _prontuarioService = new ProntuarioService(_prontuarioRepositoryMock.Object, AuditoriaDouble.Repository().Object, AuditoriaDouble.Usuario().Object);
    }

    [Fact]
    public async Task RegistrarProntuarioAsync_DadosValidosSemData_DevePreencherDataESalvar()
    {
        var novoProntuario = new Prontuario { Diagnostico = "Virose leve. Recomendada hidratação." };

        _prontuarioRepositoryMock
            .Setup(repo => repo.AdicionarAsync(It.IsAny<Prontuario>()))
            .ReturnsAsync(novoProntuario);

        var resultado = await _prontuarioService.RegistrarProntuarioAsync(novoProntuario);

        Assert.NotNull(resultado);
        Assert.True(resultado.DataRegistro.HasValue);

        _prontuarioRepositoryMock.Verify(
            repo => repo.AdicionarAsync(It.IsAny<Prontuario>()),
            Times.Once);
    }

    [Fact]
    public async Task RegistrarProntuarioAsync_DiagnosticoVazio_DeveLancarArgumentException()
    {
        var prontuarioInvalido = new Prontuario
        {
            Diagnostico = "",
            DataRegistro = DateTime.Now
        };

        var excecao = await Assert.ThrowsAsync<ArgumentException>(
            () => _prontuarioService.RegistrarProntuarioAsync(prontuarioInvalido));

        Assert.Equal(
            "O Diagnóstico é obrigatório para registrar um prontuário.",
            excecao.Message);

        _prontuarioRepositoryMock.Verify(
            repo => repo.AdicionarAsync(It.IsAny<Prontuario>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ProntuarioInexistente_DeveLancarNotFoundException()
    {
        _prontuarioRepositoryMock.Setup(repo => repo.ObterPorIdAsync(15)).ReturnsAsync((Prontuario?)null);

        var excecao = await Assert.ThrowsAsync<ClyvoVet.Domain.Exceptions.NotFoundException>(() =>
            _prontuarioService.AtualizarAsync(15, new Prontuario { Id = 15, Diagnostico = "Otite" }));

        Assert.Equal("Prontuário não encontrado para atualização.", excecao.Message);
        _prontuarioRepositoryMock.Verify(repo => repo.AtualizarAsync(It.IsAny<Prontuario>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_ProntuarioInexistente_DeveLancarNotFoundException()
    {
        _prontuarioRepositoryMock.Setup(repo => repo.ObterPorIdAsync(16)).ReturnsAsync((Prontuario?)null);

        var excecao = await Assert.ThrowsAsync<ClyvoVet.Domain.Exceptions.NotFoundException>(() =>
            _prontuarioService.ExcluirAsync(16));

        Assert.Equal("Prontuário não encontrado para exclusão.", excecao.Message);
    }
}