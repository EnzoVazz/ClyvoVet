namespace ClyvoVet.Domain.Entities;

public class Veterinario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Crmv { get; set; } = string.Empty;
    public string? Telefone { get; set; }
}