using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IProntuarioRepository
{
    Task<Prontuario> AdicionarAsync(Prontuario prontuario);
    Task<Prontuario?> ObterPorIdAsync(int id);
    Task<PagedResult<Prontuario>> ListarAsync(PageQuery query, string? termoDiagnostico);
    Task<Prontuario> AtualizarAsync(Prontuario prontuario);
    Task RemoverAsync(Prontuario prontuario);
}
