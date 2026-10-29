using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class VeterinarioService : AppService
{
    private readonly IVeterinarioRepository _veterinarioRepository;

    public VeterinarioService(IVeterinarioRepository veterinarioRepository, IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
        : base(auditoria, usuarioAtual)
    {
        _veterinarioRepository = veterinarioRepository;
    }

    public Task<PagedResult<Veterinario>> ListarAsync(PageQuery query)
        => _veterinarioRepository.ListarAsync(query);

    public async Task<Veterinario> ObterPorIdAsync(int id)
    {
        return await _veterinarioRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Veterinário não encontrado.");
    }

    public async Task<Veterinario> ObterPorCrmvAsync(string crmv)
    {
        return await _veterinarioRepository.ObterPorCrmvAsync(crmv)
            ?? throw new NotFoundException("Nenhum veterinário encontrado com este CRMV.");
    }

    public async Task<Veterinario> CadastrarVeterinarioAsync(Veterinario veterinario)
    {
        if (string.IsNullOrWhiteSpace(veterinario.Nome) || string.IsNullOrWhiteSpace(veterinario.Crmv))
            throw new ArgumentException("O Nome e o CRMV do Veterinário são obrigatórios.");

        if (await _veterinarioRepository.ExisteCrmvAsync(veterinario.Crmv))
            throw new InvalidOperationException("Já existe um veterinário cadastrado com este CRMV.");

        var salvo = await _veterinarioRepository.AdicionarAsync(veterinario);
        await AuditarAsync("Veterinario", "Cadastro", salvo.Nome);
        return salvo;
    }

    public async Task<Veterinario> AtualizarAsync(int id, Veterinario dados)
    {
        GarantirMesmoId(id, dados.Id);

        if (string.IsNullOrWhiteSpace(dados.Nome) || string.IsNullOrWhiteSpace(dados.Crmv))
            throw new ArgumentException("O Nome e o CRMV do Veterinário são obrigatórios.");

        if (await _veterinarioRepository.ObterPorIdAsync(id) is null)
            throw new NotFoundException("Veterinário não encontrado para atualização.");

        var outro = await _veterinarioRepository.ObterPorCrmvAsync(dados.Crmv);
        if (outro is not null && outro.Id != id)
            throw new InvalidOperationException("Já existe um veterinário cadastrado com este CRMV.");

        dados.Id = id;
        var atualizado = await _veterinarioRepository.AtualizarAsync(dados);
        await AuditarAsync("Veterinario", "Atualizacao", atualizado.Nome);
        return atualizado;
    }

    public async Task ExcluirAsync(int id)
    {
        var veterinario = await _veterinarioRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Veterinário não encontrado para exclusão.");

        if (await _veterinarioRepository.PossuiConsultasAsync(id))
            throw new ConflictException("Não é possível excluir este veterinário, pois ele possui consultas vinculadas.");

        await _veterinarioRepository.RemoverAsync(veterinario);
        await AuditarAsync("Veterinario", "Exclusao", veterinario.Nome);
    }
}
