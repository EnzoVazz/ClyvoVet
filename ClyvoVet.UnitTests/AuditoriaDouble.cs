using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using Moq;

namespace ClyvoVet.UnitTests;

internal static class AuditoriaDouble
{
    public static Mock<IAuditoriaRepository> Repository()
    {
        var mock = new Mock<IAuditoriaRepository>();
        mock.Setup(repository => repository.RegistrarAsync(It.IsAny<AuditoriaRegistro>()))
            .Returns(Task.CompletedTask);
        return mock;
    }

    public static Mock<IUsuarioAtual> Usuario()
    {
        var mock = new Mock<IUsuarioAtual>();
        mock.Setup(usuario => usuario.Nome).Returns("tester");
        return mock;
    }
}
