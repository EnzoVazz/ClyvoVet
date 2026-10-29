using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Domain.Services;

namespace ClyvoVet.UnitTests;

public class ClinicaServiceTests
{
    private readonly Mock<IClinicaRepository> _clinicaRepositoryMock;
    private readonly ClinicaService _clinicaService;

    public ClinicaServiceTests()
    {
        _clinicaRepositoryMock = new Mock<IClinicaRepository>();
        _clinicaService = new ClinicaService(_clinicaRepositoryMock.Object, AuditoriaDouble.Repository().Object, AuditoriaDouble.Usuario().Object);
    }

    [Fact]
    public async Task CadastrarClinicaAsync_DadosValidosECnpjInedito_DeveRetornarClinicaSalva()
    {
        var novaClinica = new Clinica { Nome = "Vet Vida", Cnpj = "12.345.678/0001-99" };

        _clinicaRepositoryMock.Setup(repo => repo.ExisteCnpjAsync(novaClinica.Cnpj)).ReturnsAsync(false);
        _clinicaRepositoryMock.Setup(repo => repo.AdicionarAsync(It.IsAny<Clinica>())).ReturnsAsync(novaClinica);

        var resultado = await _clinicaService.CadastrarClinicaAsync(novaClinica);

        Assert.NotNull(resultado);
        Assert.Equal("Vet Vida", resultado.Nome);

        _clinicaRepositoryMock.Verify(repo => repo.ExisteCnpjAsync(novaClinica.Cnpj), Times.Once);
        _clinicaRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Clinica>()), Times.Once);
    }

    [Fact]
    public async Task CadastrarClinicaAsync_CnpjVazio_DeveLancarArgumentException()
    {
        var clinicaInvalida = new Clinica { Nome = "Pet Health", Cnpj = "" };

        var excecao = await Assert.ThrowsAsync<ArgumentException>(() => _clinicaService.CadastrarClinicaAsync(clinicaInvalida));

        Assert.Equal("O Nome e o CNPJ da Clínica são obrigatórios.", excecao.Message);

        _clinicaRepositoryMock.Verify(repo => repo.ExisteCnpjAsync(It.IsAny<string>()), Times.Never);
        _clinicaRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Clinica>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarClinicaAsync_CnpjJaExistente_DeveLancarInvalidOperationException()
    {
        var clinicaDuplicada = new Clinica { Nome = "Clyvo Vet Principal", Cnpj = "99.888.777/0001-55" };

        _clinicaRepositoryMock.Setup(repo => repo.ExisteCnpjAsync(clinicaDuplicada.Cnpj)).ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => _clinicaService.CadastrarClinicaAsync(clinicaDuplicada));

        Assert.Equal("Já existe uma clínica cadastrada com este CNPJ.", excecao.Message);

        _clinicaRepositoryMock.Verify(repo => repo.ExisteCnpjAsync(clinicaDuplicada.Cnpj), Times.Once);
        _clinicaRepositoryMock.Verify(repo => repo.AdicionarAsync(It.IsAny<Clinica>()), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_ClinicaComConsultas_DeveLancarConflictException()
    {
        _clinicaRepositoryMock.Setup(repo => repo.ObterPorIdAsync(8)).ReturnsAsync(new Clinica { Id = 8, Nome = "Vet Vida" });
        _clinicaRepositoryMock.Setup(repo => repo.PossuiConsultasAsync(8)).ReturnsAsync(true);

        var excecao = await Assert.ThrowsAsync<ClyvoVet.Domain.Exceptions.ConflictException>(() => _clinicaService.ExcluirAsync(8));

        Assert.Equal("Não é possível excluir esta clínica, pois ela possui consultas vinculadas.", excecao.Message);
        _clinicaRepositoryMock.Verify(repo => repo.RemoverAsync(It.IsAny<Clinica>()), Times.Never);
    }
}
