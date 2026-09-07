using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class PetService
{
    private readonly IPetRepository _petRepository;

    public PetService(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<Pet> CadastrarPetAsync(Pet pet)
    {
        if (string.IsNullOrWhiteSpace(pet.Nome) || string.IsNullOrWhiteSpace(pet.Especie))
        {
            throw new ArgumentException("O Nome e a Espécie do Pet são obrigatórios.");
        }

        return await _petRepository.AdicionarAsync(pet);
    }
}