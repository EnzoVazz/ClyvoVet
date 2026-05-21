namespace ClyvoVet.Domain.Entities;

public class Tutor
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Telefone { get; set; }

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}