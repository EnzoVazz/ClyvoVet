namespace ClyvoVet.Domain.Entities;

public class Consulta
{
    public int Id { get; set; }
    public int IdVeterinario { get; set; }
    public int IdPet { get; set; }
    public int IdClinica { get; set; }
    public int? IdProntuario { get; set; }
    public DateTime DataConsulta { get; set; }
    public string? Status { get; set; }

    public Veterinario? Veterinario { get; set; }
    public Pet? Pet { get; set; }
    public Clinica? Clinica { get; set; }
    public Prontuario? Prontuario { get; set; }
}