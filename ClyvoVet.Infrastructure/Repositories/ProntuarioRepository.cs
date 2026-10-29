using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public class ProntuarioRepository : EfRepository<Prontuario>, IProntuarioRepository
{
    public ProntuarioRepository(ClyvoVetContext context) : base(context)
    {
    }

    public Task<Prontuario> AtualizarAsync(Prontuario prontuario) => base.AtualizarAsync(prontuario, prontuario.Id);

    public Task<PagedResult<Prontuario>> ListarAsync(PageQuery query, string? termoDiagnostico)
    {
        var prontuarios = Set.AsNoTracking();
        var termo = !string.IsNullOrWhiteSpace(termoDiagnostico) ? termoDiagnostico : query.Search;

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var busca = termo.Trim().ToLower();
            prontuarios = prontuarios.Where(prontuario =>
                prontuario.Diagnostico != null && prontuario.Diagnostico.ToLower().Contains(busca));
        }

        return prontuarios.ToPagedAsync(
            query,
            nameof(Prontuario.Id),
            nameof(Prontuario.Id),
            nameof(Prontuario.DataRegistro),
            nameof(Prontuario.Diagnostico));
    }
}
