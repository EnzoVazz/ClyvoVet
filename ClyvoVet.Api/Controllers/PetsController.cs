using ClyvoVet.Domain.Entities;
using ClyvoVet.Infrastructure.Context; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoVet.Api.Controllers;

[ApiController]
[Route("api/animais")] 
[Tags("Gestão de Pets")] 
public class PetsController : ControllerBase
{
    private readonly ClyvoVetContext _context;

    public PetsController(ClyvoVetContext context)
    {
        _context = context;
    }

    
    
    // GET 

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista todos os pets",
        Description = "Retorna uma listagem completa de todos os animais cadastrados no banco de dados da clínica."
    )]
    public async Task<ActionResult<IEnumerable<Pet>>> GetPets()
    {
        return Ok(await _context.Pets.ToListAsync()); 
    }

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Busca um pet por ID",
        Description = "Informe o ID numérico do pet para retornar seus dados completos."
    )]
    public async Task<ActionResult<Pet>> GetPetById(int id)
    {
        var pet = await _context.Pets.FindAsync(id);

        if (pet == null)
            return NotFound(new { message = "Pet não encontrado." }); 

        return Ok(pet); 
    }

    [HttpGet("especie/{especie}")]
    [SwaggerOperation(
        Summary = "Busca pets por Espécie",
        Description = "Filtra os animais pela espécie informada (ex: Cachorro, Gato)."
    )]
    public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByEspecie(string especie)
    {
        var pets = await _context.Pets
            .Where(p => p.Especie.ToLower() == especie.ToLower())
            .ToListAsync();

        if (!pets.Any())
            return NotFound(new { message = $"Nenhum pet da espécie '{especie}' encontrado." }); 

        return Ok(pets); 
    }

    [HttpGet("tutor/{tutorId}")]
    [SwaggerOperation(
        Summary = "Busca pets pelo ID do Tutor",
        Description = "Retorna todos os animais vinculados a um tutor específico."
    )]
    public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByTutor(int tutorId)
    {
        var pets = await _context.Pets
            .Where(p => p.IdTutor == tutorId)
            .ToListAsync();

        if (!pets.Any())
            return NotFound(new { message = $"Nenhum pet encontrado para o Tutor de ID {tutorId}." }); 

        return Ok(pets); 
    }

    
    
    // POST  
    
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastra um novo pet",
        Description = "Insere um novo animal no sistema. O Nome, a Espécie e o ID do Tutor são obrigatórios."
    )]
    public async Task<ActionResult<Pet>> PostPet([FromBody] Pet pet)
    {
        if (string.IsNullOrWhiteSpace(pet.Nome) || string.IsNullOrWhiteSpace(pet.Especie))
            return BadRequest(new { message = "O Nome e a Espécie do Pet são obrigatórios." }); 

        _context.Pets.Add(pet);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPetById), new { id = pet.Id }, pet);
    }

    
    
    // PUT 
    
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Atualiza os dados de um pet",
        Description = "Altera as informações de um pet existente. O ID informado na URL deve ser igual ao ID no corpo da requisição."
    )]
    public async Task<IActionResult> PutPet(int id, [FromBody] Pet petAtualizado)
    {
        if (id != petAtualizado.Id)
            return BadRequest(new { message = "O ID da URL não confere com o ID do corpo da requisição." }); 

        var petExistente = await _context.Pets.FindAsync(id);
        
        if (petExistente == null)
            return NotFound(new { message = "Pet não encontrado para atualização." }); 

        petExistente.IdTutor = petAtualizado.IdTutor;
        petExistente.Nome = petAtualizado.Nome;
        petExistente.Especie = petAtualizado.Especie;
        petExistente.Cor = petAtualizado.Cor;
        petExistente.Idade = petAtualizado.Idade;
        petExistente.Peso = petAtualizado.Peso;

        _context.Entry(petExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent(); 
    }
    
    
    // DELETE 
  
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Exclui um pet",
        Description = "Remove permanentemente um pet do banco de dados com base no ID informado."
    )]
    public async Task<IActionResult> DeletePet(int id)
    {
        var pet = await _context.Pets.FindAsync(id);
        
        if (pet == null)
            return NotFound(new { message = "Pet não encontrado para exclusão." }); 

        try
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
            return NoContent(); 
        }
        catch (DbUpdateException ex)
        {
            // Intercepta o erro ORA-02292 (Foreign Key) do Oracle
            if (ex.InnerException != null && ex.InnerException.Message.Contains("ORA-02292"))
            {
                return Conflict(new { message = "Não é possível excluir este pet, pois ele possui consultas vinculadas no histórico." });
            }
            
            throw; // Repassa qualquer outro erro não previsto
        }
    }
}