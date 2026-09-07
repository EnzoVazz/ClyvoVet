using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IConsultaRepository
{
    Task<Consulta> AdicionarAsync(Consulta consulta);
}