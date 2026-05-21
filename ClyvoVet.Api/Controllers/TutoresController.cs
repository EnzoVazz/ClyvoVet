using ClyvoVet.Domain.Entities;
using ClyvoVet.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[ApiController]
[Route("api/tutores")]
[Tags("Gestão de Tutores")]
public class TutoresController : ControllerBase
{
    private readonly ClyvoVetContext _context;

    public TutoresController(ClyvoVetContext context)
    {
        _context = context;
    }
    
    
    // GET 
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os tutores", 
        Description = "Retorna uma listagem completa dos responsáveis (tutores) cadastrados."
    )]
    public async Task<ActionResult<IEnumerable<Tutor>>> GetTutores()
    {
        return Ok(await _context.Tutores.ToListAsync());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca um tutor por ID", 
        Description = "Retorna os dados de um tutor específico baseado no ID numérico."
    )]
    public async Task<ActionResult<Tutor>> GetTutorById(int id)
    {
        var tutor = await _context.Tutores.FindAsync(id);
        
        if (tutor == null) 
            return NotFound(new { message = "Tutor não encontrado." });
            
        return Ok(tutor);
    }

    [HttpGet("cpf/{cpf}")]
    [SwaggerOperation(
        Summary = "Busca um tutor pelo CPF", 
        Description = "Retorna os dados do tutor correspondente ao CPF exato informado."
    )]
    public async Task<ActionResult<Tutor>> GetTutorByCpf(string cpf)
    {
        var tutor = await _context.Tutores.FirstOrDefaultAsync(t => t.Cpf == cpf);
        
        if (tutor == null) 
            return NotFound(new { message = "Nenhum tutor encontrado com este CPF." });
            
        return Ok(tutor);
    }

    
    
    // POST 

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra um novo tutor", 
        Description = "Insere um novo responsável no sistema. Nome e CPF são obrigatórios."
    )]
    public async Task<ActionResult<Tutor>> PostTutor([FromBody] Tutor tutor)
    {
        if (string.IsNullOrWhiteSpace(tutor.Nome) || string.IsNullOrWhiteSpace(tutor.Cpf))
            return BadRequest(new { message = "O Nome e o CPF do Tutor são obrigatórios." });

        // Validação para não quebrar a regra UNIQUE do banco de dados
        var cpfExiste = await _context.Tutores.AnyAsync(t => t.Cpf == tutor.Cpf);
        if(cpfExiste) 
            return BadRequest(new { message = "Já existe um tutor cadastrado com este CPF." });

        _context.Tutores.Add(tutor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTutorById), new { id = tutor.Id }, tutor);
    }

  
    
    // PUT

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de um tutor", 
        Description = "Altera as informações de um tutor existente. O ID da URL deve bater com o ID do corpo."
    )]
    public async Task<IActionResult> PutTutor(int id, [FromBody] Tutor tutorAtualizado)
    {
        if (id != tutorAtualizado.Id) 
            return BadRequest(new { message = "O ID da URL não confere com o ID do corpo da requisição." });

        var tutorExistente = await _context.Tutores.FindAsync(id);
        
        if (tutorExistente == null) 
            return NotFound(new { message = "Tutor não encontrado para atualização." });
        
        tutorExistente.Nome = tutorAtualizado.Nome;
        tutorExistente.Cpf = tutorAtualizado.Cpf;
        tutorExistente.Telefone = tutorAtualizado.Telefone;

        _context.Entry(tutorExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    
    
    // DELETE 

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui um tutor", 
        Description = "Remove permanentemente um tutor do banco de dados. Atenção: falhará se houver pets vinculados."
    )]
    public async Task<IActionResult> DeleteTutor(int id)
    {
        var tutor = await _context.Tutores.FindAsync(id);
        
        if (tutor == null) 
            return NotFound(new { message = "Tutor não encontrado para exclusão." });

        _context.Tutores.Remove(tutor);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}