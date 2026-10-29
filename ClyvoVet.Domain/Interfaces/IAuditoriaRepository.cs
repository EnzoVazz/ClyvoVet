using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IAuditoriaRepository
{
    Task RegistrarAsync(AuditoriaRegistro registro);
    Task<PagedResult<AuditoriaRegistro>> ListarAsync(PageQuery query, string? entidade, string? operacao);
}
