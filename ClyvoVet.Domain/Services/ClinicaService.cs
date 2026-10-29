using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class ClinicaService : AppService
{
    private readonly IClinicaRepository _clinicaRepository;

    public ClinicaService(IClinicaRepository clinicaRepository, IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
        : base(auditoria, usuarioAtual)
    {
        _clinicaRepository = clinicaRepository;
    }

    public Task<PagedResult<Clinica>> ListarAsync(PageQuery query)
        => _clinicaRepository.ListarAsync(query);

    public async Task<Clinica> ObterPorIdAsync(int id)
    {
        return await _clinicaRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Clínica não encontrada.");
    }

    public async Task<Clinica> ObterPorCnpjAsync(string cnpj)
    {
        return await _clinicaRepository.ObterPorCnpjAsync(cnpj)
            ?? throw new NotFoundException("Nenhuma clínica encontrada com este CNPJ.");
    }

    public async Task<Clinica> CadastrarClinicaAsync(Clinica clinica)
    {
        if (string.IsNullOrWhiteSpace(clinica.Nome) || string.IsNullOrWhiteSpace(clinica.Cnpj))
            throw new ArgumentException("O Nome e o CNPJ da Clínica são obrigatórios.");

        if (await _clinicaRepository.ExisteCnpjAsync(clinica.Cnpj))
            throw new InvalidOperationException("Já existe uma clínica cadastrada com este CNPJ.");

        var salva = await _clinicaRepository.AdicionarAsync(clinica);
        await AuditarAsync("Clinica", "Cadastro", salva.Nome);
        return salva;
    }

    public async Task<Clinica> AtualizarAsync(int id, Clinica dados)
    {
        GarantirMesmoId(id, dados.Id);

        if (string.IsNullOrWhiteSpace(dados.Nome) || string.IsNullOrWhiteSpace(dados.Cnpj))
            throw new ArgumentException("O Nome e o CNPJ da Clínica são obrigatórios.");

        if (await _clinicaRepository.ObterPorIdAsync(id) is null)
            throw new NotFoundException("Clínica não encontrada para atualização.");

        var outra = await _clinicaRepository.ObterPorCnpjAsync(dados.Cnpj);
        if (outra is not null && outra.Id != id)
            throw new InvalidOperationException("Já existe uma clínica cadastrada com este CNPJ.");

        dados.Id = id;
        var atualizada = await _clinicaRepository.AtualizarAsync(dados);
        await AuditarAsync("Clinica", "Atualizacao", atualizada.Nome);
        return atualizada;
    }

    public async Task ExcluirAsync(int id)
    {
        var clinica = await _clinicaRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Clínica não encontrada para exclusão.");

        if (await _clinicaRepository.PossuiConsultasAsync(id))
            throw new ConflictException("Não é possível excluir esta clínica, pois ela possui consultas vinculadas.");

        await _clinicaRepository.RemoverAsync(clinica);
        await AuditarAsync("Clinica", "Exclusao", clinica.Nome);
    }
}
