using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IPetRepository
{
    Task<Pet> AdicionarAsync(Pet pet);
    Task<Pet?> ObterPorIdAsync(int id);
    Task<PagedResult<Pet>> ListarAsync(PageQuery query, string? especie, int? idTutor);
    Task<Pet> AtualizarAsync(Pet pet);
    Task RemoverAsync(Pet pet);
    Task<bool> PossuiConsultasAsync(int petId);
}
