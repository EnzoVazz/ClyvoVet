using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class ConsultaService : AppService
{
    private readonly IConsultaRepository _consultaRepository;

    public ConsultaService(IConsultaRepository consultaRepository, IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
        : base(auditoria, usuarioAtual)
    {
        _consultaRepository = consultaRepository;
    }

    public Task<PagedResult<Consulta>> ListarAsync(PageQuery query, string? status, int? idPet)
        => _consultaRepository.ListarAsync(query, status, idPet);

    public async Task<Consulta> ObterPorIdAsync(int id)
    {
        return await _consultaRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Consulta não encontrada.");
    }

    public async Task<Consulta> AgendarConsultaAsync(Consulta consulta)
    {
        ValidarVinculos(consulta);

        if (string.IsNullOrWhiteSpace(consulta.Status))
            consulta.Status = "Agendada";

        LimparNavegacoes(consulta);
        var salva = await _consultaRepository.AdicionarAsync(consulta);
        await AuditarAsync("Consulta", "Cadastro", salva.Status);
        return salva;
    }

    public async Task<Consulta> AtualizarAsync(int id, Consulta dados)
    {
        GarantirMesmoId(id, dados.Id);
        ValidarVinculos(dados);

        if (await _consultaRepository.ObterPorIdAsync(id) is null)
            throw new NotFoundException("Consulta não encontrada para atualização.");

        if (string.IsNullOrWhiteSpace(dados.Status))
            dados.Status = "Agendada";

        dados.Id = id;
        LimparNavegacoes(dados);
        var atualizada = await _consultaRepository.AtualizarAsync(dados);
        await AuditarAsync("Consulta", "Atualizacao", atualizada.Status);
        return atualizada;
    }

    public async Task ExcluirAsync(int id)
    {
        var consulta = await _consultaRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Consulta não encontrada para exclusão.");

        await _consultaRepository.RemoverAsync(consulta);
        await AuditarAsync("Consulta", "Exclusao", consulta.Status);
    }

    private static void ValidarVinculos(Consulta consulta)
    {
        if (consulta.IdVeterinario <= 0 || consulta.IdPet <= 0 || consulta.IdClinica <= 0)
            throw new ArgumentException("Os IDs do Veterinário, Pet e Clínica são obrigatórios para agendar uma consulta.");
    }

    private static void LimparNavegacoes(Consulta consulta)
    {
        consulta.Veterinario = null;
        consulta.Pet = null;
        consulta.Clinica = null;
        consulta.Prontuario = null;
    }
}
