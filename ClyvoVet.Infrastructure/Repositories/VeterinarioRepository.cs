using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public class VeterinarioRepository : EfRepository<Veterinario>, IVeterinarioRepository
{
    public VeterinarioRepository(ClyvoVetContext context) : base(context)
    {
    }

    public Task<Veterinario> AtualizarAsync(Veterinario veterinario) => base.AtualizarAsync(veterinario, veterinario.Id);

    public Task<bool> ExisteCrmvAsync(string crmv)
        => Set.AnyAsync(veterinario => veterinario.Crmv == crmv);

    public Task<Veterinario?> ObterPorCrmvAsync(string crmv)
        => Set.AsNoTracking().FirstOrDefaultAsync(veterinario => veterinario.Crmv == crmv);

    public Task<PagedResult<Veterinario>> ListarAsync(PageQuery query)
    {
        var veterinarios = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var busca = query.Search.ToLower();
            veterinarios = veterinarios.Where(veterinario =>
                veterinario.Nome.ToLower().Contains(busca) || veterinario.Crmv.ToLower().Contains(busca));
        }

        return veterinarios.ToPagedAsync(
            query,
            nameof(Veterinario.Id),
            nameof(Veterinario.Id),
            nameof(Veterinario.Nome),
            nameof(Veterinario.Crmv));
    }

    public Task<bool> PossuiConsultasAsync(int veterinarioId)
        => Context.Consultas.AnyAsync(consulta => consulta.IdVeterinario == veterinarioId);
}
