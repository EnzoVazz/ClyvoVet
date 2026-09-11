using Microsoft.AspNetCore.Mvc.Testing;

namespace ClyvoVet.IntegrationTests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
}

[CollectionDefinition("ClyvoVet API Collection")]
public class ApiCollection : ICollectionFixture<ApiWebApplicationFactory>
{
}