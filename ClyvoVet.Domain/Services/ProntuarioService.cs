using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class ProntuarioService : AppService
{
    private readonly IProntuarioRepository _prontuarioRepository;

    public ProntuarioService(IProntuarioRepository prontuarioRepository, IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
        : base(auditoria, usuarioAtual)
    {
        _prontuarioRepository = prontuarioRepository;
    }

    public Task<PagedResult<Prontuario>> ListarAsync(PageQuery query, string? termoDiagnostico)
        => _prontuarioRepository.ListarAsync(query, termoDiagnostico);

    public async Task<Prontuario> ObterPorIdAsync(int id)
    {
        return await _prontuarioRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Prontuário não encontrado.");
    }

    public async Task<Prontuario> RegistrarProntuarioAsync(Prontuario prontuario)
    {
        if (string.IsNullOrWhiteSpace(prontuario.Diagnostico))
            throw new ArgumentException("O Diagnóstico é obrigatório para registrar um prontuário.");

        if (!prontuario.DataRegistro.HasValue)
            prontuario.DataRegistro = DateTime.Now;

        var salvo = await _prontuarioRepository.AdicionarAsync(prontuario);
        await AuditarAsync("Prontuario", "Cadastro", salvo.Diagnostico);
        return salvo;
    }

    public async Task<Prontuario> AtualizarAsync(int id, Prontuario dados)
    {
        GarantirMesmoId(id, dados.Id);

        if (string.IsNullOrWhiteSpace(dados.Diagnostico))
            throw new ArgumentException("O Diagnóstico é obrigatório para registrar um prontuário.");

        if (await _prontuarioRepository.ObterPorIdAsync(id) is null)
            throw new NotFoundException("Prontuário não encontrado para atualização.");

        if (!dados.DataRegistro.HasValue)
            dados.DataRegistro = DateTime.Now;

        dados.Id = id;
        var atualizado = await _prontuarioRepository.AtualizarAsync(dados);
        await AuditarAsync("Prontuario", "Atualizacao", atualizado.Diagnostico);
        return atualizado;
    }

    public async Task ExcluirAsync(int id)
    {
        var prontuario = await _prontuarioRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Prontuário não encontrado para exclusão.");

        await _prontuarioRepository.RemoverAsync(prontuario);
        await AuditarAsync("Prontuario", "Exclusao", prontuario.Diagnostico);
    }
}
