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
[Route("api/animais")]
[Tags("Gestão de Pets")]
public class PetsController : ControllerBase
{
    private readonly PetService _petService;
    private readonly HateoasFactory _hateoas;

    public PetsController(PetService petService, HateoasFactory hateoas)
    {
        _petService = petService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetPets")]
    [SwaggerOperation(
        Summary = "Lista pets com paginação",
        Description = "Retorna os animais paginados. Filtros: search, especie e idTutor. Ordenação: sortBy (Id, Nome, Especie, Idade, Peso, IdTutor) e sortDirection (asc ou desc)."
    )]
    public async Task<ActionResult<PagedResponse<Pet>>> GetPets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null,
        [FromQuery] string? especie = null,
        [FromQuery] int? idTutor = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _petService.ListarAsync(query, especie, idTutor);
        return Ok(_hateoas.Page(result, PetLinks));
    }

    [HttpGet("{id:int}", Name = "GetPetById")]
    [SwaggerOperation(Summary = "Busca um pet por ID", Description = "Informe o ID numérico do pet para retornar seus dados e os links HATEOAS.")]
    public async Task<ActionResult<ResourceResponse<Pet>>> GetPetById(int id)
    {
        var pet = await _petService.ObterPorIdAsync(id);
        return Ok(_hateoas.Resource(pet, PetLinks(pet).ToArray()));
    }

    [HttpGet("especie/{especie}", Name = "GetPetsByEspecie")]
    [SwaggerOperation(Summary = "Busca pets por espécie", Description = "Filtra os animais pela espécie informada, com paginação e ordenação.")]
    public Task<ActionResult<PagedResponse<Pet>>> GetPetsByEspecie(
        string especie,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
        => GetPets(page, pageSize, sortBy, sortDirection, search, especie, null);

    [HttpGet("tutor/{tutorId:int}", Name = "GetPetsByTutor")]
    [SwaggerOperation(Summary = "Busca pets pelo ID do tutor", Description = "Retorna os animais vinculados a um tutor, com paginação e ordenação.")]
    public Task<ActionResult<PagedResponse<Pet>>> GetPetsByTutor(
        int tutorId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null)
        => GetPets(page, pageSize, sortBy, sortDirection, search, null, tutorId);

    [Authorize(Roles = Papeis.Operacao)]
    [HttpPost(Name = "PostPet")]
    [SwaggerOperation(Summary = "Cadastra um novo pet", Description = "Insere um novo animal. Nome, espécie e ID do tutor são obrigatórios. Roles: Admin, Veterinario e Recepcionista.")]
    public async Task<ActionResult<ResourceResponse<Pet>>> PostPet([FromBody] Pet pet)
    {
        var salvo = await _petService.CadastrarPetAsync(pet);
        var resource = _hateoas.Resource(salvo, PetLinks(salvo).ToArray());
        return CreatedAtAction(nameof(GetPetById), new { id = salvo.Id }, resource);
    }

    [Authorize(Roles = Papeis.Operacao)]
    [HttpPut("{id:int}", Name = "PutPet")]
    [SwaggerOperation(Summary = "Atualiza os dados de um pet", Description = "Altera um pet existente. O ID da URL deve ser igual ao ID do corpo.")]
    public async Task<IActionResult> PutPet(int id, [FromBody] Pet pet)
    {
        await _petService.AtualizarAsync(id, pet);
        return NoContent();
    }

    [Authorize(Roles = Papeis.Clinica)]
    [HttpDelete("{id:int}", Name = "DeletePet")]
    [SwaggerOperation(Summary = "Exclui um pet", Description = "Remove um pet. A exclusão é recusada quando existem consultas vinculadas. Roles: Admin e Veterinario.")]
    public async Task<IActionResult> DeletePet(int id)
    {
        await _petService.ExcluirAsync(id);
        return NoContent();
    }

    private IReadOnlyList<LinkDto> PetLinks(Pet pet)
    {
        return
        [
            _hateoas.Link("self", "GetPetById", "GET", new { id = pet.Id }),
            _hateoas.Link("update", "PutPet", "PUT", new { id = pet.Id }),
            _hateoas.Link("delete", "DeletePet", "DELETE", new { id = pet.Id }),
            _hateoas.Link("tutor", "GetTutorById", "GET", new { id = pet.IdTutor }),
            _hateoas.Link("consultas", "GetConsultasByPet", "GET", new { petId = pet.Id })
        ];
    }
}
