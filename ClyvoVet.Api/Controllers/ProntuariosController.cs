using ClyvoVet.Domain.Entities;
using ClyvoVet.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[ApiController]
[Route("api/prontuarios")]
[Tags("Gestão de Prontuários")]
public class ProntuariosController : ControllerBase
{
    private readonly ClyvoVetContext _context;

    public ProntuariosController(ClyvoVetContext context)
    {
        _context = context;
    }
    
    
    
    // GET 
    

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os prontuários", 
        Description = "Retorna o histórico completo de todos os prontuários cadastrados."
    )]
    public async Task<ActionResult<IEnumerable<Prontuario>>> GetProntuarios()
    {
        return Ok(await _context.Prontuarios.ToListAsync());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca um prontuário por ID", 
        Description = "Retorna os detalhes de um prontuário específico pelo seu ID numérico."
    )]
    public async Task<ActionResult<Prontuario>> GetProntuarioById(int id)
    {
        var prontuario = await _context.Prontuarios.FindAsync(id);
        
        if (prontuario == null) 
            return NotFound(new { message = "Prontuário não encontrado." });
            
        return Ok(prontuario);
    }

    [HttpGet("diagnostico/{termo}")]
    [SwaggerOperation(
        Summary = "Busca prontuários por palavra-chave no diagnóstico", 
        Description = "Filtra prontuários que contenham um termo específico no texto do diagnóstico (ex: 'virose')."
    )]
    public async Task<ActionResult<IEnumerable<Prontuario>>> GetProntuariosByDiagnostico(string termo)
    {
        var prontuarios = await _context.Prontuarios
            .Where(p => p.Diagnostico != null && p.Diagnostico.ToLower().Contains(termo.ToLower()))
            .ToListAsync();

        if (!prontuarios.Any()) 
            return NotFound(new { message = $"Nenhum prontuário encontrado contendo a palavra '{termo}'." });
            
        return Ok(prontuarios);
    }

    
    
    // POST

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cria um novo prontuário", 
        Description = "Registra um novo prontuário no sistema. Se a data de registro não for enviada, assumirá a data e hora atual."
    )]
    public async Task<ActionResult<Prontuario>> PostProntuario([FromBody] Prontuario prontuario)
    {
        if (string.IsNullOrWhiteSpace(prontuario.Diagnostico))
            return BadRequest(new { message = "O Diagnóstico não pode estar vazio." });

        // Se o usuário não enviou a data, preenchemos com a data de hoje automaticamente
        if (prontuario.DataRegistro == null)
        {
            prontuario.DataRegistro = DateTime.Now;
        }

        _context.Prontuarios.Add(prontuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProntuarioById), new { id = prontuario.Id }, prontuario);
    }

    
    
    // PUT

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza um prontuário", 
        Description = "Altera o texto do diagnóstico ou a data de um prontuário existente."
    )]
    public async Task<IActionResult> PutProntuario(int id, [FromBody] Prontuario prontuarioAtualizado)
    {
        if (id != prontuarioAtualizado.Id) 
            return BadRequest(new { message = "O ID da URL não confere com o ID do corpo da requisição." });

        var prontuarioExistente = await _context.Prontuarios.FindAsync(id);
        
        if (prontuarioExistente == null) 
            return NotFound(new { message = "Prontuário não encontrado para atualização." });

        // Atualizando os dados
        prontuarioExistente.Diagnostico = prontuarioAtualizado.Diagnostico;
        prontuarioExistente.DataRegistro = prontuarioAtualizado.DataRegistro;

        _context.Entry(prontuarioExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    
    
    // DELETE

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui um prontuário", 
        Description = "Remove permanentemente um prontuário do banco de dados."
    )]
    public async Task<IActionResult> DeleteProntuario(int id)
    {
        var prontuario = await _context.Prontuarios.FindAsync(id);
        
        if (prontuario == null) 
            return NotFound(new { message = "Prontuário não encontrado para exclusão." });

        _context.Prontuarios.Remove(prontuario);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}