using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;

namespace ClyvoVet.Domain.Interfaces;

public interface IClinicaRepository
{
    Task<bool> ExisteCnpjAsync(string cnpj);
    Task<Clinica> AdicionarAsync(Clinica clinica);
    Task<Clinica?> ObterPorIdAsync(int id);
    Task<Clinica?> ObterPorCnpjAsync(string cnpj);
    Task<PagedResult<Clinica>> ListarAsync(PageQuery query);
    Task<Clinica> AtualizarAsync(Clinica clinica);
    Task RemoverAsync(Clinica clinica);
    Task<bool> PossuiConsultasAsync(int clinicaId);
}
