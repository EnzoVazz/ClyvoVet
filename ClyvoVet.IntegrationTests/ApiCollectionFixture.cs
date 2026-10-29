using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClyvoVet.IntegrationTests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }
}

[CollectionDefinition("ClyvoVet API Collection")]
public class ApiCollection : ICollectionFixture<ApiWebApplicationFactory>
{
}
