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
[Route("api/veterinarios")]
[Tags("Gestão de Veterinários")]
public class VeterinariosController : ControllerBase
{
    private readonly VeterinarioService _veterinarioService;
    private readonly HateoasFactory _hateoas;

    public VeterinariosController(VeterinarioService veterinarioService, HateoasFactory hateoas)
    {
        _veterinarioService = veterinarioService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetVeterinarios")]
    [SwaggerOperation(
        Summary = "Lista veterinários com paginação",
        Description = "Retorna os profissionais paginados. Filtro search em nome e CRMV. Ordenação: sortBy (Id, Nome, Crmv) e sortDirection (asc ou desc)."
    )]
    public async Task<ActionResult<PagedResponse<Veterinario>>> GetVeterinarios(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _veterinarioService.ListarAsync(query);
        return Ok(_hateoas.Page(result, VeterinarioLinks));
    }

    [HttpGet("{id:int}", Name = "GetVeterinarioById")]
    [SwaggerOperation(Summary = "Busca um veterinário por ID", Description = "Retorna os dados de um veterinário e os links HATEOAS.")]
    public async Task<ActionResult<ResourceResponse<Veterinario>>> GetVeterinarioById(int id)
    {
        var veterinario = await _veterinarioService.ObterPorIdAsync(id);
        return Ok(_hateoas.Resource(veterinario, VeterinarioLinks(veterinario).ToArray()));
    }

    [HttpGet("crmv/{crmv}", Name = "GetVeterinarioByCrmv")]
    [SwaggerOperation(Summary = "Busca um veterinário pelo CRMV", Description = "Retorna o profissional correspondente ao CRMV informado.")]
    public async Task<ActionResult<ResourceResponse<Veterinario>>> GetVeterinarioByCrmv(string crmv)
    {
        var veterinario = await _veterinarioService.ObterPorCrmvAsync(crmv);
        return Ok(_hateoas.Resource(veterinario, VeterinarioLinks(veterinario).ToArray()));
    }

    [Authorize(Roles = Papeis.Gestao)]
    [HttpPost(Name = "PostVeterinario")]
    [SwaggerOperation(Summary = "Cadastra um novo veterinário", Description = "Nome e CRMV são obrigatórios. O CRMV não pode se repetir. Role: Admin.")]
    public async Task<ActionResult<ResourceResponse<Veterinario>>> PostVeterinario([FromBody] Veterinario veterinario)
    {
        var salvo = await _veterinarioService.CadastrarVeterinarioAsync(veterinario);
        var resource = _hateoas.Resource(salvo, VeterinarioLinks(salvo).ToArray());
        return CreatedAtAction(nameof(GetVeterinarioById), new { id = salvo.Id }, resource);
    }

    [Authorize(Roles = Papeis.Gestao)]
    [HttpPut("{id:int}", Name = "PutVeterinario")]
    [SwaggerOperation(Summary = "Atualiza os dados de um veterinário", Description = "Altera um profissional existente. O ID da URL deve bater com o ID do corpo. Role: Admin.")]
    public async Task<IActionResult> PutVeterinario(int id, [FromBody] Veterinario veterinario)
    {
        await _veterinarioService.AtualizarAsync(id, veterinario);
        return NoContent();
    }

    [Authorize(Roles = Papeis.Gestao)]
    [HttpDelete("{id:int}", Name = "DeleteVeterinario")]
    [SwaggerOperation(Summary = "Exclui um veterinário", Description = "Remove um veterinário. A exclusão é recusada quando existem consultas vinculadas. Role: Admin.")]
    public async Task<IActionResult> DeleteVeterinario(int id)
    {
        await _veterinarioService.ExcluirAsync(id);
        return NoContent();
    }

    private IReadOnlyList<LinkDto> VeterinarioLinks(Veterinario veterinario)
    {
        return
        [
            _hateoas.Link("self", "GetVeterinarioById", "GET", new { id = veterinario.Id }),
            _hateoas.Link("update", "PutVeterinario", "PUT", new { id = veterinario.Id }),
            _hateoas.Link("delete", "DeleteVeterinario", "DELETE", new { id = veterinario.Id })
        ];
    }
}
