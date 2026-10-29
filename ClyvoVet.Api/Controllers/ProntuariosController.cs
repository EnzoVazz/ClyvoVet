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
[Route("api/prontuarios")]
[Tags("Gestão de Prontuários")]
public class ProntuariosController : ControllerBase
{
    private readonly ProntuarioService _prontuarioService;
    private readonly HateoasFactory _hateoas;

    public ProntuariosController(ProntuarioService prontuarioService, HateoasFactory hateoas)
    {
        _prontuarioService = prontuarioService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetProntuarios")]
    [SwaggerOperation(
        Summary = "Lista prontuários com paginação",
        Description = "Retorna o histórico paginado. Filtro search no diagnóstico. Ordenação: sortBy (Id, DataRegistro, Diagnostico) e sortDirection (asc ou desc)."
    )]
    public async Task<ActionResult<PagedResponse<Prontuario>>> GetProntuarios(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _prontuarioService.ListarAsync(query, null);
        return Ok(_hateoas.Page(result, ProntuarioLinks));
    }

    [HttpGet("{id:int}", Name = "GetProntuarioById")]
    [SwaggerOperation(Summary = "Busca um prontuário por ID", Description = "Retorna os detalhes de um prontuário e os links HATEOAS.")]
    public async Task<ActionResult<ResourceResponse<Prontuario>>> GetProntuarioById(int id)
    {
        var prontuario = await _prontuarioService.ObterPorIdAsync(id);
        return Ok(_hateoas.Resource(prontuario, ProntuarioLinks(prontuario).ToArray()));
    }

    [HttpGet("diagnostico/{termo}", Name = "GetProntuariosByDiagnostico")]
    [SwaggerOperation(Summary = "Busca prontuários por palavra-chave", Description = "Filtra diagnósticos que contenham o termo informado, com paginação.")]
    public async Task<ActionResult<PagedResponse<Prontuario>>> GetProntuariosByDiagnostico(
        string termo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, null);
        var result = await _prontuarioService.ListarAsync(query, termo);
        return Ok(_hateoas.Page(result, ProntuarioLinks));
    }

    [Authorize(Roles = Papeis.Clinica)]
    [HttpPost(Name = "PostProntuario")]
    [SwaggerOperation(Summary = "Cria um novo prontuário", Description = "O diagnóstico é obrigatório. Sem data, o registro assume a data atual. Roles: Admin e Veterinario.")]
    public async Task<ActionResult<ResourceResponse<Prontuario>>> PostProntuario([FromBody] Prontuario prontuario)
    {
        var salvo = await _prontuarioService.RegistrarProntuarioAsync(prontuario);
        var resource = _hateoas.Resource(salvo, ProntuarioLinks(salvo).ToArray());
        return CreatedAtAction(nameof(GetProntuarioById), new { id = salvo.Id }, resource);
    }

    [Authorize(Roles = Papeis.Clinica)]
    [HttpPut("{id:int}", Name = "PutProntuario")]
    [SwaggerOperation(Summary = "Atualiza um prontuário", Description = "Altera o diagnóstico ou a data de um prontuário existente. Roles: Admin e Veterinario.")]
    public async Task<IActionResult> PutProntuario(int id, [FromBody] Prontuario prontuario)
    {
        await _prontuarioService.AtualizarAsync(id, prontuario);
        return NoContent();
    }

    [Authorize(Roles = Papeis.Clinica)]
    [HttpDelete("{id:int}", Name = "DeleteProntuario")]
    [SwaggerOperation(Summary = "Exclui um prontuário", Description = "Remove um prontuário do histórico. Roles: Admin e Veterinario.")]
    public async Task<IActionResult> DeleteProntuario(int id)
    {
        await _prontuarioService.ExcluirAsync(id);
        return NoContent();
    }

    private IReadOnlyList<LinkDto> ProntuarioLinks(Prontuario prontuario)
    {
        return
        [
            _hateoas.Link("self", "GetProntuarioById", "GET", new { id = prontuario.Id }),
            _hateoas.Link("update", "PutProntuario", "PUT", new { id = prontuario.Id }),
            _hateoas.Link("delete", "DeleteProntuario", "DELETE", new { id = prontuario.Id })
        ];
    }
}
