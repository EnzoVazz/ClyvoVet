using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public abstract class AppService
{
    private readonly IAuditoriaRepository _auditoria;
    private readonly IUsuarioAtual _usuarioAtual;

    protected AppService(IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
    {
        _auditoria = auditoria;
        _usuarioAtual = usuarioAtual;
    }

    protected Task AuditarAsync(string entidade, string operacao, string? detalhe)
    {
        return _auditoria.RegistrarAsync(new AuditoriaRegistro
        {
            Entidade = entidade,
            Operacao = operacao,
            Usuario = string.IsNullOrWhiteSpace(_usuarioAtual.Nome) ? "sistema" : _usuarioAtual.Nome,
            Detalhe = detalhe,
            DataUtc = DateTime.UtcNow
        });
    }

    protected static void GarantirMesmoId(int idRota, int idCorpo)
    {
        if (idCorpo != 0 && idRota != idCorpo)
            throw new ArgumentException("O ID da URL não confere com o ID do corpo da requisição.");
    }
}
