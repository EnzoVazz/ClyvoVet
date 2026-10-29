namespace ClyvoVet.Domain.Entities;

public class AuditoriaRegistro
{
    public string? Id { get; set; }
    public string Entidade { get; set; } = string.Empty;
    public string Operacao { get; set; } = string.Empty;
    public string? Usuario { get; set; }
    public string? Detalhe { get; set; }
    public DateTime DataUtc { get; set; } = DateTime.UtcNow;
}
