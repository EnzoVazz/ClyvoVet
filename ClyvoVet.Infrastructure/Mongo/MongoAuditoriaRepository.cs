using System.Text.RegularExpressions;
using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ClyvoVet.Infrastructure.Mongo;

public class MongoAuditoriaRepository : IAuditoriaRepository
{
    private readonly IMongoCollection<AuditoriaDocumento> _collection;

    public MongoAuditoriaRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<AuditoriaDocumento>("auditorias");
    }

    public async Task RegistrarAsync(AuditoriaRegistro registro)
    {
        var documento = new AuditoriaDocumento
        {
            Entidade = registro.Entidade,
            Operacao = registro.Operacao,
            Usuario = registro.Usuario,
            Detalhe = registro.Detalhe,
            DataUtc = registro.DataUtc == default ? DateTime.UtcNow : registro.DataUtc
        };

        await _collection.InsertOneAsync(documento);
        registro.Id = documento.Id;
    }

    public async Task<PagedResult<AuditoriaRegistro>> ListarAsync(PageQuery query, string? entidade, string? operacao)
    {
        var filter = Builders<AuditoriaDocumento>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(entidade))
            filter &= Builders<AuditoriaDocumento>.Filter.Regex(
                documento => documento.Entidade,
                new BsonRegularExpression($"^{Regex.Escape(entidade.Trim())}$", "i"));

        if (!string.IsNullOrWhiteSpace(operacao))
            filter &= Builders<AuditoriaDocumento>.Filter.Regex(
                documento => documento.Operacao,
                new BsonRegularExpression($"^{Regex.Escape(operacao.Trim())}$", "i"));

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var busca = new BsonRegularExpression(Regex.Escape(query.Search), "i");
            filter &= Builders<AuditoriaDocumento>.Filter.Or(
                Builders<AuditoriaDocumento>.Filter.Regex(documento => documento.Detalhe, busca),
                Builders<AuditoriaDocumento>.Filter.Regex(documento => documento.Usuario, busca),
                Builders<AuditoriaDocumento>.Filter.Regex(documento => documento.Entidade, busca));
        }

        var sort = MontarOrdenacao(query);
        var total = await _collection.CountDocumentsAsync(filter);
        var documentos = await _collection.Find(filter)
            .Sort(sort)
            .Skip((query.Page - 1) * query.PageSize)
            .Limit(query.PageSize)
            .ToListAsync();

        return new PagedResult<AuditoriaRegistro>
        {
            Items = documentos.Select(Mapear).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = (int)total
        };
    }

    private static SortDefinition<AuditoriaDocumento> MontarOrdenacao(PageQuery query)
    {
        var campo = string.IsNullOrWhiteSpace(query.SortBy) ? "DataUtc" : query.SortBy;
        var descendente = string.IsNullOrWhiteSpace(query.SortBy)
            || string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return campo.ToLower() switch
        {
            "entidade" => descendente
                ? Builders<AuditoriaDocumento>.Sort.Descending(documento => documento.Entidade)
                : Builders<AuditoriaDocumento>.Sort.Ascending(documento => documento.Entidade),
            "operacao" => descendente
                ? Builders<AuditoriaDocumento>.Sort.Descending(documento => documento.Operacao)
                : Builders<AuditoriaDocumento>.Sort.Ascending(documento => documento.Operacao),
            "datautc" => descendente
                ? Builders<AuditoriaDocumento>.Sort.Descending(documento => documento.DataUtc)
                : Builders<AuditoriaDocumento>.Sort.Ascending(documento => documento.DataUtc),
            _ => throw new ArgumentException("Ordenação de auditoria aceita apenas DataUtc, Entidade ou Operacao.")
        };
    }

    private static AuditoriaRegistro Mapear(AuditoriaDocumento documento)
    {
        return new AuditoriaRegistro
        {
            Id = documento.Id,
            Entidade = documento.Entidade,
            Operacao = documento.Operacao,
            Usuario = documento.Usuario,
            Detalhe = documento.Detalhe,
            DataUtc = documento.DataUtc
        };
    }
}
