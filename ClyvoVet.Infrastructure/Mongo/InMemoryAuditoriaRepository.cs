using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Infrastructure.Mongo;

public class InMemoryAuditoriaRepository : IAuditoriaRepository
{
    private readonly List<AuditoriaRegistro> _registros = [];
    private readonly Lock _gate = new();

    public Task RegistrarAsync(AuditoriaRegistro registro)
    {
        lock (_gate)
        {
            _registros.Add(new AuditoriaRegistro
            {
                Id = Guid.NewGuid().ToString("N"),
                Entidade = registro.Entidade,
                Operacao = registro.Operacao,
                Usuario = registro.Usuario,
                Detalhe = registro.Detalhe,
                DataUtc = registro.DataUtc == default ? DateTime.UtcNow : registro.DataUtc
            });
        }

        return Task.CompletedTask;
    }

    public Task<PagedResult<AuditoriaRegistro>> ListarAsync(PageQuery query, string? entidade, string? operacao)
    {
        lock (_gate)
        {
            IEnumerable<AuditoriaRegistro> consulta = _registros;

            if (!string.IsNullOrWhiteSpace(entidade))
                consulta = consulta.Where(registro => registro.Entidade.Equals(entidade.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(operacao))
                consulta = consulta.Where(registro => registro.Operacao.Equals(operacao.Trim(), StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var busca = query.Search.ToLower();
                consulta = consulta.Where(registro =>
                    (registro.Detalhe != null && registro.Detalhe.ToLower().Contains(busca))
                    || (registro.Usuario != null && registro.Usuario.ToLower().Contains(busca))
                    || registro.Entidade.ToLower().Contains(busca));
            }

            var campo = string.IsNullOrWhiteSpace(query.SortBy) ? "DataUtc" : query.SortBy;
            var descendente = string.IsNullOrWhiteSpace(query.SortBy)
                || string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            consulta = campo.ToLower() switch
            {
                "entidade" => descendente
                    ? consulta.OrderByDescending(registro => registro.Entidade)
                    : consulta.OrderBy(registro => registro.Entidade),
                "operacao" => descendente
                    ? consulta.OrderByDescending(registro => registro.Operacao)
                    : consulta.OrderBy(registro => registro.Operacao),
                "datautc" => descendente
                    ? consulta.OrderByDescending(registro => registro.DataUtc)
                    : consulta.OrderBy(registro => registro.DataUtc),
                _ => throw new ArgumentException("Ordenação de auditoria aceita apenas DataUtc, Entidade ou Operacao.")
            };

            var total = consulta.Count();
            var itens = consulta
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<AuditoriaRegistro>
            {
                Items = itens,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total
            });
        }
    }
}
