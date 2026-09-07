using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IClinicaRepository
{
    Task<bool> ExisteCnpjAsync(string cnpj);
    Task<Clinica> AdicionarAsync(Clinica clinica);
}