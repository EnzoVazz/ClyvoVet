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
[Route("api/tutores")]
[Tags("Gestão de Tutores")]
public class TutoresController : ControllerBase
{
    private readonly TutorService _tutorService;
    private readonly HateoasFactory _hateoas;

    public TutoresController(TutorService tutorService, HateoasFactory hateoas)
    {
        _tutorService = tutorService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetTutores")]
    [SwaggerOperation(
        Summary = "Lista tutores com paginação",
        Description = "Retorna os responsáveis paginados. Filtro search em nome e CPF. Ordenação: sortBy (Id, Nome, Cpf) e sortDirection (asc ou desc)."
    )]
    public async Task<ActionResult<PagedResponse<Tutor>>> GetTutores(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _tutorService.ListarAsync(query);
        return Ok(_hateoas.Page(result, TutorLinks));
    }

    [HttpGet("{id:int}", Name = "GetTutorById")]
    [SwaggerOperation(Summary = "Busca um tutor por ID", Description = "Retorna os dados de um tutor e os links HATEOAS.")]
    public async Task<ActionResult<ResourceResponse<Tutor>>> GetTutorById(int id)
    {
        var tutor = await _tutorService.ObterPorIdAsync(id);
        return Ok(_hateoas.Resource(tutor, TutorLinks(tutor).ToArray()));
    }

    [HttpGet("cpf/{cpf}", Name = "GetTutorByCpf")]
    [SwaggerOperation(Summary = "Busca um tutor pelo CPF", Description = "Retorna o tutor correspondente ao CPF exato informado.")]
    public async Task<ActionResult<ResourceResponse<Tutor>>> GetTutorByCpf(string cpf)
    {
        var tutor = await _tutorService.ObterPorCpfAsync(cpf);
        return Ok(_hateoas.Resource(tutor, TutorLinks(tutor).ToArray()));
    }

    [Authorize(Roles = Papeis.Operacao)]
    [HttpPost(Name = "PostTutor")]
    [SwaggerOperation(Summary = "Cadastra um novo tutor", Description = "Nome e CPF são obrigatórios. O CPF não pode se repetir. Roles: Admin, Veterinario e Recepcionista.")]
    public async Task<ActionResult<ResourceResponse<Tutor>>> PostTutor([FromBody] Tutor tutor)
    {
        var salvo = await _tutorService.CadastrarTutorAsync(tutor);
        var resource = _hateoas.Resource(salvo, TutorLinks(salvo).ToArray());
        return CreatedAtAction(nameof(GetTutorById), new { id = salvo.Id }, resource);
    }

    [Authorize(Roles = Papeis.Operacao)]
    [HttpPut("{id:int}", Name = "PutTutor")]
    [SwaggerOperation(Summary = "Atualiza os dados de um tutor", Description = "Altera um tutor existente. O ID da URL deve bater com o ID do corpo.")]
    public async Task<IActionResult> PutTutor(int id, [FromBody] Tutor tutor)
    {
        await _tutorService.AtualizarAsync(id, tutor);
        return NoContent();
    }

    [Authorize(Roles = Papeis.Clinica)]
    [HttpDelete("{id:int}", Name = "DeleteTutor")]
    [SwaggerOperation(Summary = "Exclui um tutor", Description = "Remove um tutor. A exclusão é recusada quando existem pets vinculados. Roles: Admin e Veterinario.")]
    public async Task<IActionResult> DeleteTutor(int id)
    {
        await _tutorService.ExcluirAsync(id);
        return NoContent();
    }

    private IReadOnlyList<LinkDto> TutorLinks(Tutor tutor)
    {
        return
        [
            _hateoas.Link("self", "GetTutorById", "GET", new { id = tutor.Id }),
            _hateoas.Link("update", "PutTutor", "PUT", new { id = tutor.Id }),
            _hateoas.Link("delete", "DeleteTutor", "DELETE", new { id = tutor.Id }),
            _hateoas.Link("pets", "GetPetsByTutor", "GET", new { tutorId = tutor.Id })
        ];
    }
}
