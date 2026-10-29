using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface ITutorRepository
{
    Task<bool> ExisteCpfAsync(string cpf);
    Task<Tutor> AdicionarAsync(Tutor tutor);
    Task<Tutor?> ObterPorIdAsync(int id);
    Task<Tutor?> ObterPorCpfAsync(string cpf);
    Task<PagedResult<Tutor>> ListarAsync(PageQuery query);
    Task<Tutor> AtualizarAsync(Tutor tutor);
    Task RemoverAsync(Tutor tutor);
    Task<bool> PossuiPetsAsync(int tutorId);
}
