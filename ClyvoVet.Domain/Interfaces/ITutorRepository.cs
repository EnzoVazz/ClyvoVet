using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface ITutorRepository
{
    Task<bool> ExisteCpfAsync(string cpf);
    Task<Tutor> AdicionarAsync(Tutor tutor);
}