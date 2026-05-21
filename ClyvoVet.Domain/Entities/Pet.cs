namespace ClyvoVet.Domain.Entities;

public class Pet
{
    public int Id { get; set; }
    public int IdTutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Cor { get; set; }
    public int? Idade { get; set; }
    public decimal? Peso { get; set; }

    public Tutor? Tutor { get; set; }
    public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
}