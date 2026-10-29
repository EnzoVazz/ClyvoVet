using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClyvoVet.IntegrationTests;

public static class AuthHelper
{
    public static async Task<HttpClient> CreateClientAsync(
        ApiWebApplicationFactory factory,
        string username = "admin",
        string password = "Admin@123")
    {
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        login.EnsureSuccessStatusCode();

        var payload = await login.Content.ReadFromJsonAsync<JsonElement>();
        var token = payload.GetProperty("token").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
