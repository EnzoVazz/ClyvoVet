using ClyvoVet.Domain.Entities;
using ClyvoVet.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[ApiController]
[Route("api/clinicas")]
[Tags("Gestão de Clínicas")]
public class ClinicasController : ControllerBase
{
    private readonly ClyvoVetContext _context;

    public ClinicasController(ClyvoVetContext context)
    {
        _context = context;
    }

    
    
    // GET 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todas as clínicas", 
        Description = "Retorna uma listagem completa das clínicas veterinárias parceiras cadastradas."
    )]
    public async Task<ActionResult<IEnumerable<Clinica>>> GetClinicas()
    {
        return Ok(await _context.Clinicas.ToListAsync());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca uma clínica por ID", 
        Description = "Retorna os dados de uma clínica específica baseada no seu ID numérico."
    )]
    public async Task<ActionResult<Clinica>> GetClinicaById(int id)
    {
        var clinica = await _context.Clinicas.FindAsync(id);
        
        if (clinica == null) 
            return NotFound(new { message = "Clínica não encontrada." });
            
        return Ok(clinica);
    }

    [HttpGet("cnpj/{*cnpj}")] 
    [SwaggerOperation(
        Summary = "Busca uma clínica pelo CNPJ", 
        Description = "Retorna os dados da clínica correspondente ao CNPJ exato informado."
    )]
    public async Task<ActionResult<Clinica>> GetClinicaByCnpj(string cnpj)
    {
        // Decodifica a string caso o Swagger envie a barra como %2F
        var cnpjDecodificado = Uri.UnescapeDataString(cnpj);

        var clinica = await _context.Clinicas.FirstOrDefaultAsync(c => c.Cnpj == cnpjDecodificado);
        
        if (clinica == null) 
            return NotFound(new { message = "Nenhuma clínica encontrada com este CNPJ." });
            
        return Ok(clinica);
    }
    
    
    // POST 
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra uma nova clínica", 
        Description = "Insere uma nova clínica no sistema. O Nome e o CNPJ são obrigatórios."
    )]
    public async Task<ActionResult<Clinica>> PostClinica([FromBody] Clinica clinica)
    {
        if (string.IsNullOrWhiteSpace(clinica.Nome) || string.IsNullOrWhiteSpace(clinica.Cnpj))
            return BadRequest(new { message = "O Nome e o CNPJ da Clínica são obrigatórios." });

        // Validação para não quebrar a regra UNIQUE do banco de dados
        var cnpjExiste = await _context.Clinicas.AnyAsync(c => c.Cnpj == clinica.Cnpj);
        if(cnpjExiste) 
            return BadRequest(new { message = "Já existe uma clínica cadastrada com este CNPJ." });

        _context.Clinicas.Add(clinica);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClinicaById), new { id = clinica.Id }, clinica);
    }

    
    
    // PUT 

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de uma clínica", 
        Description = "Altera as informações de uma clínica existente. O ID da URL deve bater com o ID do corpo da requisição."
    )]
    public async Task<IActionResult> PutClinica(int id, [FromBody] Clinica clinicaAtualizada)
    {
        if (id != clinicaAtualizada.Id) 
            return BadRequest(new { message = "O ID da URL não confere com o ID do corpo da requisição." });

        var clinicaExistente = await _context.Clinicas.FindAsync(id);
        
        if (clinicaExistente == null) 
            return NotFound(new { message = "Clínica não encontrada para atualização." });

        // Atualizando os dados
        clinicaExistente.Nome = clinicaAtualizada.Nome;
        clinicaExistente.Cnpj = clinicaAtualizada.Cnpj;
        clinicaExistente.Logradouro = clinicaAtualizada.Logradouro;

        _context.Entry(clinicaExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    // DELETE 

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui uma clínica", 
        Description = "Remove permanentemente uma clínica do banco de dados."
    )]
    public async Task<IActionResult> DeleteClinica(int id)
    {
        var clinica = await _context.Clinicas.FindAsync(id);
        
        if (clinica == null) 
            return NotFound(new { message = "Clínica não encontrada para exclusão." });

        _context.Clinicas.Remove(clinica);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}