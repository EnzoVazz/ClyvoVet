using ClyvoVet.Api.Auth;
using ClyvoVet.Api.Hateoas;
using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/consultas")]
[Tags("Gestão de Consultas")]
public class ConsultasController : ControllerBase
{
    private readonly ConsultaService _consultaService;
    private readonly HateoasFactory _hateoas;

    public ConsultasController(ConsultaService consultaService, HateoasFactory hateoas)
    {
        _consultaService = consultaService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetConsultas")]
    [SwaggerOperation(
        Summary = "Lista consultas com paginação",
        Description = "Retorna a agenda paginada. Filtros: status, idPet e search. Ordenação: sortBy (Id, DataConsulta, Status, IdPet, IdVeterinario, IdClinica) e sortDirection (asc ou desc)."
    )]
    public async Task<ActionResult<PagedResponse<Consulta>>> GetConsultas(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] int? idPet = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _consultaService.ListarAsync(query, status, idPet);
        return Ok(_hateoas.Page(result, ConsultaLinks));
    }

    [HttpGet("{id:int}", Name = "GetConsultaById")]
    [SwaggerOperation(Summary = "Busca uma consulta por ID", Description = "Retorna os detalhes de uma consulta e os links HATEOAS para pet, veterinário, clínica e prontuário.")]
    public async Task<ActionResult<ResourceResponse<Consulta>>> GetConsultaById(int id)
    {
        var consulta = await _consultaService.ObterPorIdAsync(id);
        return Ok(_hateoas.Resource(consulta, ConsultaLinks(consulta).ToArray()));
    }

    [HttpGet("status/{status}", Name = "GetConsultasByStatus")]
    [SwaggerOperation(Summary = "Busca consultas pelo status", Description = "Filtra a agenda pelo status (Agendada, Concluida, Cancelada), com paginação.")]
    public Task<ActionResult<PagedResponse<Consulta>>> GetConsultasByStatus(
        string status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
        => GetConsultas(page, pageSize, sortBy, sortDirection, search, status, null);

    [HttpGet("pet/{petId:int}", Name = "GetConsultasByPet")]
    [SwaggerOperation(Summary = "Busca o histórico de consultas de um pet", Description = "Retorna as consultas vinculadas ao pet, com paginação.")]
    public Task<ActionResult<PagedResponse<Consulta>>> GetConsultasByPet(
        int petId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
        => GetConsultas(page, pageSize, sortBy, sortDirection, search, null, petId);

    [Authorize(Roles = Papeis.Operacao)]
    [HttpPost(Name = "PostConsulta")]
    [SwaggerOperation(Summary = "Agenda uma nova consulta", Description = "Os IDs de veterinário, pet e clínica são obrigatórios. Sem status, a consulta nasce como Agendada. Roles: Admin, Veterinario e Recepcionista.")]
    public async Task<ActionResult<ResourceResponse<Consulta>>> PostConsulta([FromBody] Consulta consulta)
    {
        var salva = await _consultaService.AgendarConsultaAsync(consulta);
        var resource = _hateoas.Resource(salva, ConsultaLinks(salva).ToArray());
        return CreatedAtAction(nameof(GetConsultaById), new { id = salva.Id }, resource);
    }

    [Authorize(Roles = Papeis.Operacao)]
    [HttpPut("{id:int}", Name = "PutConsulta")]
    [SwaggerOperation(Summary = "Atualiza os dados de uma consulta", Description = "Altera status, data ou vínculos de um agendamento. Roles: Admin, Veterinario e Recepcionista.")]
    public async Task<IActionResult> PutConsulta(int id, [FromBody] Consulta consulta)
    {
        await _consultaService.AtualizarAsync(id, consulta);
        return NoContent();
    }

    [Authorize(Roles = Papeis.Clinica)]
    [HttpDelete("{id:int}", Name = "DeleteConsulta")]
    [SwaggerOperation(Summary = "Exclui uma consulta", Description = "Remove um agendamento. Roles: Admin e Veterinario.")]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        await _consultaService.ExcluirAsync(id);
        return NoContent();
    }

    private IReadOnlyList<LinkDto> ConsultaLinks(Consulta consulta)
    {
        var links = new List<LinkDto>
        {
            _hateoas.Link("self", "GetConsultaById", "GET", new { id = consulta.Id }),
            _hateoas.Link("update", "PutConsulta", "PUT", new { id = consulta.Id }),
            _hateoas.Link("delete", "DeleteConsulta", "DELETE", new { id = consulta.Id }),
            _hateoas.Link("pet", "GetPetById", "GET", new { id = consulta.IdPet }),
            _hateoas.Link("veterinario", "GetVeterinarioById", "GET", new { id = consulta.IdVeterinario }),
            _hateoas.Link("clinica", "GetClinicaById", "GET", new { id = consulta.IdClinica })
        };

        if (consulta.IdProntuario is int prontuarioId)
            links.Add(_hateoas.Link("prontuario", "GetProntuarioById", "GET", new { id = prontuarioId }));

        return links;
    }
}
