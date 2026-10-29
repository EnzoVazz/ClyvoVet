using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public class TutorRepository : EfRepository<Tutor>, ITutorRepository
{
    public TutorRepository(ClyvoVetContext context) : base(context)
    {
    }

    public Task<Tutor> AtualizarAsync(Tutor tutor) => base.AtualizarAsync(tutor, tutor.Id);

    public Task<bool> ExisteCpfAsync(string cpf)
        => Set.AnyAsync(tutor => tutor.Cpf == cpf);

    public Task<Tutor?> ObterPorCpfAsync(string cpf)
        => Set.AsNoTracking().FirstOrDefaultAsync(tutor => tutor.Cpf == cpf);

    public Task<PagedResult<Tutor>> ListarAsync(PageQuery query)
    {
        var tutores = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var busca = query.Search.ToLower();
            tutores = tutores.Where(tutor =>
                tutor.Nome.ToLower().Contains(busca) || tutor.Cpf.ToLower().Contains(busca));
        }

        return tutores.ToPagedAsync(
            query,
            nameof(Tutor.Id),
            nameof(Tutor.Id),
            nameof(Tutor.Nome),
            nameof(Tutor.Cpf));
    }

    public Task<bool> PossuiPetsAsync(int tutorId)
        => Context.Pets.AnyAsync(pet => pet.IdTutor == tutorId);
}
