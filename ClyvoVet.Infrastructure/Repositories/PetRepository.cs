using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public class PetRepository : EfRepository<Pet>, IPetRepository
{
    public PetRepository(ClyvoVetContext context) : base(context)
    {
    }

    public Task<Pet> AtualizarAsync(Pet pet) => base.AtualizarAsync(pet, pet.Id);

    public Task<PagedResult<Pet>> ListarAsync(PageQuery query, string? especie, int? idTutor)
    {
        var pets = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(especie))
        {
            var filtro = especie.Trim().ToLower();
            pets = pets.Where(pet => pet.Especie.ToLower() == filtro);
        }

        if (idTutor.HasValue)
            pets = pets.Where(pet => pet.IdTutor == idTutor.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var busca = query.Search.ToLower();
            pets = pets.Where(pet => pet.Nome.ToLower().Contains(busca) || pet.Especie.ToLower().Contains(busca));
        }

        return pets.ToPagedAsync(
            query,
            nameof(Pet.Id),
            nameof(Pet.Id),
            nameof(Pet.Nome),
            nameof(Pet.Especie),
            nameof(Pet.Idade),
            nameof(Pet.Peso),
            nameof(Pet.IdTutor));
    }

    public Task<bool> PossuiConsultasAsync(int petId)
        => Context.Consultas.AnyAsync(consulta => consulta.IdPet == petId);
}
