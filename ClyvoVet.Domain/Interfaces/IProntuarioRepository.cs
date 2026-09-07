using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IProntuarioRepository
{
    Task<Prontuario> AdicionarAsync(Prontuario prontuario);
}