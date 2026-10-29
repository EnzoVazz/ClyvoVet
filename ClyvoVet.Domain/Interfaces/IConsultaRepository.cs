using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IConsultaRepository
{
    Task<Consulta> AdicionarAsync(Consulta consulta);
    Task<Consulta?> ObterPorIdAsync(int id);
    Task<PagedResult<Consulta>> ListarAsync(PageQuery query, string? status, int? idPet);
    Task<Consulta> AtualizarAsync(Consulta consulta);
    Task RemoverAsync(Consulta consulta);
}
