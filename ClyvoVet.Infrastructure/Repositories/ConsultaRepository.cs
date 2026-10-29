using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using ClyvoVet.Infrastructure.Context;
using ClyvoVet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoVet.Infrastructure.Repositories;

public class ConsultaRepository : EfRepository<Consulta>, IConsultaRepository
{
    public ConsultaRepository(ClyvoVetContext context) : base(context)
    {
    }

    public Task<Consulta> AtualizarAsync(Consulta consulta) => base.AtualizarAsync(consulta, consulta.Id);

    public Task<PagedResult<Consulta>> ListarAsync(PageQuery query, string? status, int? idPet)
    {
        var consultas = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var filtro = status.Trim().ToLower();
            consultas = consultas.Where(consulta =>
                consulta.Status != null && consulta.Status.ToLower() == filtro);
        }

        if (idPet.HasValue)
            consultas = consultas.Where(consulta => consulta.IdPet == idPet.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var busca = query.Search.ToLower();
            consultas = consultas.Where(consulta =>
                consulta.Status != null && consulta.Status.ToLower().Contains(busca));
        }

        return consultas.ToPagedAsync(
            query,
            nameof(Consulta.Id),
            nameof(Consulta.Id),
            nameof(Consulta.DataConsulta),
            nameof(Consulta.Status),
            nameof(Consulta.IdPet),
            nameof(Consulta.IdVeterinario),
            nameof(Consulta.IdClinica));
    }
}
