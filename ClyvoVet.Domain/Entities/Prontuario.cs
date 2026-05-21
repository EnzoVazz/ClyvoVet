namespace ClyvoVet.Domain.Entities;

public class Prontuario
{
    public int Id { get; set; }
    public string? Diagnostico { get; set; }
    public DateTime? DataRegistro { get; set; }
}