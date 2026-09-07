using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IPetRepository
{
    Task<Pet> AdicionarAsync(Pet pet);
}