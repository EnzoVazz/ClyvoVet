using ClyvoVet.Domain.Entities;
using ClyvoVet.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[ApiController]
[Route("api/veterinarios")]
[Tags("Gestão de Veterinários")]
public class VeterinariosController : ControllerBase
{
    private readonly ClyvoVetContext _context;

    public VeterinariosController(ClyvoVetContext context)
    {
        _context = context;
    }

    
    
    // GET 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os veterinários", 
        Description = "Retorna uma listagem completa dos médicos veterinários cadastrados no sistema."
    )]
    public async Task<ActionResult<IEnumerable<Veterinario>>> GetVeterinarios()
    {
        return Ok(await _context.Veterinarios.ToListAsync());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca um veterinário por ID", 
        Description = "Retorna os dados de um veterinário específico baseado no seu ID numérico."
    )]
    public async Task<ActionResult<Veterinario>> GetVeterinarioById(int id)
    {
        var veterinario = await _context.Veterinarios.FindAsync(id);
        
        if (veterinario == null) 
            return NotFound(new { message = "Veterinário não encontrado." });
            
        return Ok(veterinario);
    }

    [HttpGet("crmv/{crmv}")]
    [SwaggerOperation(
        Summary = "Busca um veterinário pelo CRMV", 
        Description = "Retorna os dados do médico correspondente ao número de registro (CRMV) informado."
    )]
    public async Task<ActionResult<Veterinario>> GetVeterinarioByCrmv(string crmv)
    {
        var veterinario = await _context.Veterinarios.FirstOrDefaultAsync(v => v.Crmv == crmv);
        
        if (veterinario == null) 
            return NotFound(new { message = "Nenhum veterinário encontrado com este CRMV." });
            
        return Ok(veterinario);
    }

    
    
    // POST 

    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra um novo veterinário", 
        Description = "Insere um novo profissional no sistema. O Nome e o CRMV são obrigatórios."
    )]
    public async Task<ActionResult<Veterinario>> PostVeterinario([FromBody] Veterinario veterinario)
    {
        if (string.IsNullOrWhiteSpace(veterinario.Nome) || string.IsNullOrWhiteSpace(veterinario.Crmv))
            return BadRequest(new { message = "O Nome e o CRMV do Veterinário são obrigatórios." });

        // Validação para não quebrar a regra UNIQUE do banco de dados
        var crmvExiste = await _context.Veterinarios.AnyAsync(v => v.Crmv == veterinario.Crmv);
        if(crmvExiste) 
            return BadRequest(new { message = "Já existe um veterinário cadastrado com este CRMV." });

        _context.Veterinarios.Add(veterinario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetVeterinarioById), new { id = veterinario.Id }, veterinario);
    }
    
    // PUT 

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de um veterinário", 
        Description = "Altera as informações de um profissional existente. O ID da URL deve bater com o ID do corpo."
    )]
    public async Task<IActionResult> PutVeterinario(int id, [FromBody] Veterinario veterinarioAtualizado)
    {
        if (id != veterinarioAtualizado.Id) 
            return BadRequest(new { message = "O ID da URL não confere com o ID do corpo da requisição." });

        var veterinarioExistente = await _context.Veterinarios.FindAsync(id);
        
        if (veterinarioExistente == null) 
            return NotFound(new { message = "Veterinário não encontrado para atualização." });

        // Atualizando os dados
        veterinarioExistente.Nome = veterinarioAtualizado.Nome;
        veterinarioExistente.Crmv = veterinarioAtualizado.Crmv;
        veterinarioExistente.Telefone = veterinarioAtualizado.Telefone;

        _context.Entry(veterinarioExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    // DELETE

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui um veterinário", 
        Description = "Remove permanentemente um veterinário do banco de dados."
    )]
    public async Task<IActionResult> DeleteVeterinario(int id)
    {
        var veterinario = await _context.Veterinarios.FindAsync(id);
        
        if (veterinario == null) 
            return NotFound(new { message = "Veterinário não encontrado para exclusão." });

        _context.Veterinarios.Remove(veterinario);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}