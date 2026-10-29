using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace ClyvoVet.Infrastructure.Mongo;

public class MongoHealthCheck : IHealthCheck
{
    private readonly IMongoClient _client;

    public MongoHealthCheck(IMongoClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var cursor = await _client.ListDatabaseNamesAsync(cancellationToken);
            await cursor.MoveNextAsync(cancellationToken);
            return HealthCheckResult.Healthy("Conexão com o MongoDB estabelecida.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Não foi possível conectar ao MongoDB.", ex);
        }
    }
}
