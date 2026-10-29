using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public class ClinicaRepository : EfRepository<Clinica>, IClinicaRepository
{
    public ClinicaRepository(ClyvoVetContext context) : base(context)
    {
    }

    public Task<Clinica> AtualizarAsync(Clinica clinica) => base.AtualizarAsync(clinica, clinica.Id);

    public Task<bool> ExisteCnpjAsync(string cnpj)
        => Set.AnyAsync(clinica => clinica.Cnpj == cnpj);

    public Task<Clinica?> ObterPorCnpjAsync(string cnpj)
        => Set.AsNoTracking().FirstOrDefaultAsync(clinica => clinica.Cnpj == cnpj);

    public Task<PagedResult<Clinica>> ListarAsync(PageQuery query)
    {
        var clinicas = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var busca = query.Search.ToLower();
            clinicas = clinicas.Where(clinica =>
                clinica.Nome.ToLower().Contains(busca) || clinica.Cnpj.ToLower().Contains(busca));
        }

        return clinicas.ToPagedAsync(
            query,
            nameof(Clinica.Id),
            nameof(Clinica.Id),
            nameof(Clinica.Nome),
            nameof(Clinica.Cnpj));
    }

    public Task<bool> PossuiConsultasAsync(int clinicaId)
        => Context.Consultas.AnyAsync(consulta => consulta.IdClinica == clinicaId);
}
