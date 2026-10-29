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
[Route("api/auditoria")]
[Tags("Auditoria NoSQL")]
public class AuditoriaController : ControllerBase
{
    private readonly AuditoriaService _auditoriaService;
    private readonly HateoasFactory _hateoas;

    public AuditoriaController(AuditoriaService auditoriaService, HateoasFactory hateoas)
    {
        _auditoriaService = auditoriaService;
        _hateoas = hateoas;
    }

    [HttpGet(Name = "GetAuditoria")]
    [SwaggerOperation(
        Summary = "Lista a trilha de auditoria no MongoDB",
        Description = "Consulta os documentos gravados a cada cadastro, atualização e exclusão. Filtros: entidade, operacao e search. Ordenação padrão: DataUtc desc. Campos de sortBy: DataUtc, Entidade, Operacao."
    )]
    public async Task<ActionResult<PagedResponse<AuditoriaRegistro>>> GetAuditoria(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] string? search = null,
        [FromQuery] string? entidade = null,
        [FromQuery] string? operacao = null)
    {
        var query = PageQuery.Create(page, pageSize, sortBy, sortDirection, search);
        var result = await _auditoriaService.ListarAsync(query, entidade, operacao);
        return Ok(_hateoas.Page(result, _ =>
        [
            _hateoas.Link("collection", "GetAuditoria", "GET")
        ]));
    }
}
