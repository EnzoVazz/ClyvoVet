using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class AuditoriaService
{
    private readonly IAuditoriaRepository _auditoriaRepository;

    public AuditoriaService(IAuditoriaRepository auditoriaRepository)
    {
        _auditoriaRepository = auditoriaRepository;
    }

    public Task<PagedResult<AuditoriaRegistro>> ListarAsync(PageQuery query, string? entidade, string? operacao)
        => _auditoriaRepository.ListarAsync(query, entidade, operacao);
}
