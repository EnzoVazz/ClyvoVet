using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClyvoVet.Infrastructure.Mongo;

public class AuditoriaDocumento
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Entidade { get; set; } = string.Empty;
    public string Operacao { get; set; } = string.Empty;
    public string? Usuario { get; set; }
    public string? Detalhe { get; set; }
    public DateTime DataUtc { get; set; }
}
