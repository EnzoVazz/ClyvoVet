namespace ClyvoVet.Domain.Entities;

public class Clinica
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string? Logradouro { get; set; }
}