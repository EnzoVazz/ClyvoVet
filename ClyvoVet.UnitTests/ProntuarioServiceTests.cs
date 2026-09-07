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
        _prontuarioService = new ProntuarioService(_prontuarioRepositoryMock.Object);
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
}