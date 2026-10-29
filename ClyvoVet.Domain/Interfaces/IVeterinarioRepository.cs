using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IVeterinarioRepository
{
    Task<bool> ExisteCrmvAsync(string crmv);
    Task<Veterinario> AdicionarAsync(Veterinario veterinario);
    Task<Veterinario?> ObterPorIdAsync(int id);
    Task<Veterinario?> ObterPorCrmvAsync(string crmv);
    Task<PagedResult<Veterinario>> ListarAsync(PageQuery query);
    Task<Veterinario> AtualizarAsync(Veterinario veterinario);
    Task RemoverAsync(Veterinario veterinario);
    Task<bool> PossuiConsultasAsync(int veterinarioId);
}
