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
[Route("api/clinicas")]
[Tags("Gestão de Clínicas")]
public class ClinicasController : ControllerBase
{
    private readonly ClinicaService _clinicaService;
    private readonly HateoasFactory _hateoas;

    public ClinicasController(ClinicaService clinicaService, HateoasFactory hateoas)
    {
        _clinicaService = clinicaService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetClinicas")]
    [SwaggerOperation(
        Summary = "Lista clínicas com paginação",
        Description = "Retorna as clínicas paginadas. Filtro search em nome e CNPJ. Ordenação: sortBy (Id, Nome, Cnpj) e sortDirection (asc ou desc)."
    )]
    public async Task<ActionResult<PagedResponse<Clinica>>> GetClinicas(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _clinicaService.ListarAsync(query);
        return Ok(_hateoas.Page(result, ClinicaLinks));
    }

    [HttpGet("{id:int}", Name = "GetClinicaById")]
    [SwaggerOperation(Summary = "Busca uma clínica por ID", Description = "Retorna os dados de uma clínica e os links HATEOAS.")]
    public async Task<ActionResult<ResourceResponse<Clinica>>> GetClinicaById(int id)
    {
        var clinica = await _clinicaService.ObterPorIdAsync(id);
        return Ok(_hateoas.Resource(clinica, ClinicaLinks(clinica).ToArray()));
    }

    [HttpGet("cnpj/{*cnpj}", Name = "GetClinicaByCnpj")]
    [SwaggerOperation(Summary = "Busca uma clínica pelo CNPJ", Description = "Retorna a clínica correspondente ao CNPJ exato informado.")]
    public async Task<ActionResult<ResourceResponse<Clinica>>> GetClinicaByCnpj(string cnpj)
    {
        var clinica = await _clinicaService.ObterPorCnpjAsync(Uri.UnescapeDataString(cnpj));
        return Ok(_hateoas.Resource(clinica, ClinicaLinks(clinica).ToArray()));
    }

    [Authorize(Roles = Papeis.Gestao)]
    [HttpPost(Name = "PostClinica")]
    [SwaggerOperation(Summary = "Cadastra uma nova clínica", Description = "Nome e CNPJ são obrigatórios. O CNPJ não pode se repetir. Role: Admin.")]
    public async Task<ActionResult<ResourceResponse<Clinica>>> PostClinica([FromBody] Clinica clinica)
    {
        var salva = await _clinicaService.CadastrarClinicaAsync(clinica);
        var resource = _hateoas.Resource(salva, ClinicaLinks(salva).ToArray());
        return CreatedAtAction(nameof(GetClinicaById), new { id = salva.Id }, resource);
    }

    [Authorize(Roles = Papeis.Gestao)]
    [HttpPut("{id:int}", Name = "PutClinica")]
    [SwaggerOperation(Summary = "Atualiza os dados de uma clínica", Description = "Altera uma clínica existente. O ID da URL deve bater com o ID do corpo. Role: Admin.")]
    public async Task<IActionResult> PutClinica(int id, [FromBody] Clinica clinica)
    {
        await _clinicaService.AtualizarAsync(id, clinica);
        return NoContent();
    }

    [Authorize(Roles = Papeis.Gestao)]
    [HttpDelete("{id:int}", Name = "DeleteClinica")]
    [SwaggerOperation(Summary = "Exclui uma clínica", Description = "Remove uma clínica. A exclusão é recusada quando existem consultas vinculadas. Role: Admin.")]
    public async Task<IActionResult> DeleteClinica(int id)
    {
        await _clinicaService.ExcluirAsync(id);
        return NoContent();
    }

    private IReadOnlyList<LinkDto> ClinicaLinks(Clinica clinica)
    {
        return
        [
            _hateoas.Link("self", "GetClinicaById", "GET", new { id = clinica.Id }),
            _hateoas.Link("update", "PutClinica", "PUT", new { id = clinica.Id }),
            _hateoas.Link("delete", "DeleteClinica", "DELETE", new { id = clinica.Id })
        ];
    }
}
