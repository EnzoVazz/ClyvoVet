using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IVeterinarioRepository
{
    Task<bool> ExisteCrmvAsync(string crmv);
    Task<Veterinario> AdicionarAsync(Veterinario veterinario);
}