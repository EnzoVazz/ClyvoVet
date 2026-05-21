using ClyvoVet.Domain.Entities;
using ClyvoVet.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[ApiController]
[Route("api/consultas")]
[Tags("Gestão de Consultas")]
public class ConsultasController : ControllerBase
{
    private readonly ClyvoVetContext _context;

    public ConsultasController(ClyvoVetContext context)
    {
        _context = context;
    }
    
    
    // GET 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todas as consultas", 
        Description = "Retorna a agenda completa de consultas cadastradas no sistema."
    )]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultas()
    {
        return Ok(await _context.Consultas.ToListAsync());
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca uma consulta por ID", 
        Description = "Retorna os detalhes exatos de uma consulta específica."
    )]
    public async Task<ActionResult<Consulta>> GetConsultaById(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        
        if (consulta == null) 
            return NotFound(new { message = "Consulta não encontrada." });
            
        return Ok(consulta);
    }

    [HttpGet("status/{status}")]
    [SwaggerOperation(
        Summary = "Busca consultas pelo Status", 
        Description = "Filtra a agenda pelo status da consulta (ex: 'Agendada', 'Concluida', 'Cancelada')."
    )]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultasByStatus(string status)
    {
        var consultas = await _context.Consultas
            .Where(c => c.Status != null && c.Status.ToLower() == status.ToLower())
            .ToListAsync();

        if (!consultas.Any()) 
            return NotFound(new { message = $"Nenhuma consulta encontrada com o status '{status}'." });
            
        return Ok(consultas);
    }

    [HttpGet("pet/{petId}")]
    [SwaggerOperation(
        Summary = "Busca o histórico de consultas de um Pet", 
        Description = "Retorna todas as consultas vinculadas ao ID de um pet específico."
    )]
    public async Task<ActionResult<IEnumerable<Consulta>>> GetConsultasByPet(int petId)
    {
        var consultas = await _context.Consultas
            .Where(c => c.IdPet == petId)
            .ToListAsync();

        if (!consultas.Any()) 
            return NotFound(new { message = $"Nenhuma consulta encontrada para o Pet de ID {petId}." });
            
        return Ok(consultas);
    }
    
    
    
    // POST 

    [HttpPost]
    [SwaggerOperation(
        Summary = "Agenda uma nova consulta", 
        Description = "Cria um novo agendamento. É obrigatório informar os IDs do Veterinário, Pet e Clínica."
    )]
    public async Task<ActionResult<Consulta>> PostConsulta([FromBody] Consulta consulta)
    {
        // Validação básica para garantir que as chaves estrangeiras foram preenchidas
        if (consulta.IdVeterinario <= 0 || consulta.IdPet <= 0 || consulta.IdClinica <= 0)
            return BadRequest(new { message = "Os IDs do Veterinário, Pet e Clínica são obrigatórios para agendar uma consulta." });

        _context.Consultas.Add(consulta);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetConsultaById), new { id = consulta.Id }, consulta);
    }
    
    
    
    // PUT 

    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de uma consulta", 
        Description = "Altera as informações de um agendamento existente (ex: mudar o status ou adicionar um ID de Prontuário)."
    )]
    public async Task<IActionResult> PutConsulta(int id, [FromBody] Consulta consultaAtualizada)
    {
        if (id != consultaAtualizada.Id) 
            return BadRequest(new { message = "O ID da URL não confere com o ID do corpo da requisição." });

        var consultaExistente = await _context.Consultas.FindAsync(id);
        
        if (consultaExistente == null) 
            return NotFound(new { message = "Consulta não encontrada para atualização." });

        // Atualizando os dados
        consultaExistente.IdVeterinario = consultaAtualizada.IdVeterinario;
        consultaExistente.IdPet = consultaAtualizada.IdPet;
        consultaExistente.IdClinica = consultaAtualizada.IdClinica;
        consultaExistente.IdProntuario = consultaAtualizada.IdProntuario;
        consultaExistente.DataConsulta = consultaAtualizada.DataConsulta;
        consultaExistente.Status = consultaAtualizada.Status;

        _context.Entry(consultaExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    
    
    // DELETE

    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Cancela/Exclui uma consulta", 
        Description = "Remove permanentemente um agendamento do banco de dados."
    )]
    public async Task<IActionResult> DeleteConsulta(int id)
    {
        var consulta = await _context.Consultas.FindAsync(id);
        
        if (consulta == null) 
            return NotFound(new { message = "Consulta não encontrada para exclusão." });

        _context.Consultas.Remove(consulta);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}