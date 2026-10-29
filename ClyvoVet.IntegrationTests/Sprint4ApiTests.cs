using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClyvoVet.IntegrationTests;

[Collection("ClyvoVet API Collection")]
public class Sprint4ApiTests
{
    private readonly ApiWebApplicationFactory _factory;

    public Sprint4ApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTutores_SemToken_RetornaStatus401Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/tutores");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_SenhaInvalida_RetornaStatus401Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "admin", password = "errada" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostClinica_ComoRecepcionista_RetornaStatus403Forbidden()
    {
        var client = await AuthHelper.CreateClientAsync(_factory, "recepcao", "Recep@123");

        var response = await client.PostAsJsonAsync("/api/clinicas", new { nome = "Clínica Centro", cnpj = "12345678000199" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetTutores_OrdenacaoInvalida_RetornaStatus400BadRequest()
    {
        var client = await AuthHelper.CreateClientAsync(_factory);

        var response = await client.GetAsync("/api/tutores?sortBy=senha");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FluxoClinico_PaginacaoHateoasAutorizacaoEAuditoria()
    {
        var admin = await AuthHelper.CreateClientAsync(_factory);
        var sufixo = Random.Shared.Next(100000, 999999).ToString();
        var nomeTutor = $"Tutor {sufixo}";

        var tutorResponse = await admin.PostAsJsonAsync("/api/tutores", new { nome = nomeTutor, cpf = sufixo });
        Assert.Equal(HttpStatusCode.Created, tutorResponse.StatusCode);
        var tutorId = await ReadIdAsync(tutorResponse);

        var segundo = await admin.PostAsJsonAsync("/api/tutores", new { nome = $"{nomeTutor} B", cpf = $"{sufixo}9" });
        Assert.Equal(HttpStatusCode.Created, segundo.StatusCode);

        var pagina = await admin.GetAsync($"/api/tutores?search={sufixo}&page=1&pageSize=1&sortBy=nome&sortDirection=asc");
        Assert.Equal(HttpStatusCode.OK, pagina.StatusCode);
        var paginaJson = await pagina.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(2, paginaJson.GetProperty("totalCount").GetInt32());
        Assert.Equal(2, paginaJson.GetProperty("totalPages").GetInt32());
        Assert.Equal(1, paginaJson.GetProperty("items").GetArrayLength());
        Assert.Contains(paginaJson.GetProperty("_links").EnumerateArray(), link => link.GetProperty("rel").GetString() == "next");
        Assert.Contains(paginaJson.GetProperty("items")[0].GetProperty("_links").EnumerateArray(), link => link.GetProperty("rel").GetString() == "self");

        var petResponse = await admin.PostAsJsonAsync("/api/animais", new { nome = $"Rex {sufixo}", especie = "Cachorro", idTutor = tutorId });
        Assert.Equal(HttpStatusCode.Created, petResponse.StatusCode);
        var petId = await ReadIdAsync(petResponse);

        var pet = await admin.GetAsync($"/api/animais/{petId}");
        var petJson = await pet.Content.ReadFromJsonAsync<JsonElement>();
        var relacoes = petJson.GetProperty("_links").EnumerateArray().Select(link => link.GetProperty("rel").GetString()).ToArray();
        Assert.Contains("tutor", relacoes);
        Assert.Contains("consultas", relacoes);

        var clinicaResponse = await admin.PostAsJsonAsync("/api/clinicas", new { nome = $"Clinica {sufixo}", cnpj = sufixo });
        var clinicaId = await ReadIdAsync(clinicaResponse);
        var vetResponse = await admin.PostAsJsonAsync("/api/veterinarios", new { nome = $"Vet {sufixo}", crmv = $"CRMV-{sufixo}" });
        var vetId = await ReadIdAsync(vetResponse);
        var prontuarioResponse = await admin.PostAsJsonAsync("/api/prontuarios", new { diagnostico = $"Virose {sufixo}" });
        var prontuarioId = await ReadIdAsync(prontuarioResponse);

        var consultaResponse = await admin.PostAsJsonAsync("/api/consultas", new
        {
            idVeterinario = vetId,
            idPet = petId,
            idClinica = clinicaId,
            idProntuario = prontuarioId,
            dataConsulta = DateTime.UtcNow.AddDays(1)
        });
        Assert.Equal(HttpStatusCode.Created, consultaResponse.StatusCode);
        var consultaJson = await consultaResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Agendada", consultaJson.GetProperty("data").GetProperty("status").GetString());

        var recepcao = await AuthHelper.CreateClientAsync(_factory, "recepcao", "Recep@123");
        var exclusaoNegada = await recepcao.DeleteAsync($"/api/animais/{petId}");
        Assert.Equal(HttpStatusCode.Forbidden, exclusaoNegada.StatusCode);

        var exclusaoPet = await admin.DeleteAsync($"/api/animais/{petId}");
        Assert.Equal(HttpStatusCode.Conflict, exclusaoPet.StatusCode);

        var auditoria = await admin.GetAsync($"/api/auditoria?entidade=Tutor&operacao=Cadastro&search={Uri.EscapeDataString(nomeTutor)}");
        Assert.Equal(HttpStatusCode.OK, auditoria.StatusCode);
        var auditoriaJson = await auditoria.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(auditoriaJson.GetProperty("totalCount").GetInt32() >= 1);
        Assert.Contains(
            auditoriaJson.GetProperty("items").EnumerateArray(),
            item => item.GetProperty("data").GetProperty("detalhe").GetString() == nomeTutor);
    }

    private static async Task<int> ReadIdAsync(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("data").GetProperty("id").GetInt32();
    }
}
