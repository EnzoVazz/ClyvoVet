using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class TutorService : AppService
{
    private readonly ITutorRepository _tutorRepository;

    public TutorService(ITutorRepository tutorRepository, IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
        : base(auditoria, usuarioAtual)
    {
        _tutorRepository = tutorRepository;
    }

    public Task<PagedResult<Tutor>> ListarAsync(PageQuery query)
        => _tutorRepository.ListarAsync(query);

    public async Task<Tutor> ObterPorIdAsync(int id)
    {
        return await _tutorRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Tutor não encontrado.");
    }

    public async Task<Tutor> ObterPorCpfAsync(string cpf)
    {
        return await _tutorRepository.ObterPorCpfAsync(cpf)
            ?? throw new NotFoundException("Nenhum tutor encontrado com este CPF.");
    }

    public async Task<Tutor> CadastrarTutorAsync(Tutor tutor)
    {
        if (string.IsNullOrWhiteSpace(tutor.Nome) || string.IsNullOrWhiteSpace(tutor.Cpf))
            throw new ArgumentException("O Nome e o CPF do Tutor são obrigatórios.");

        if (await _tutorRepository.ExisteCpfAsync(tutor.Cpf))
            throw new InvalidOperationException("Já existe um tutor cadastrado com este CPF.");

        tutor.Pets.Clear();
        var salvo = await _tutorRepository.AdicionarAsync(tutor);
        await AuditarAsync("Tutor", "Cadastro", salvo.Nome);
        return salvo;
    }

    public async Task<Tutor> AtualizarAsync(int id, Tutor dados)
    {
        GarantirMesmoId(id, dados.Id);

        if (string.IsNullOrWhiteSpace(dados.Nome) || string.IsNullOrWhiteSpace(dados.Cpf))
            throw new ArgumentException("O Nome e o CPF do Tutor são obrigatórios.");

        if (await _tutorRepository.ObterPorIdAsync(id) is null)
            throw new NotFoundException("Tutor não encontrado para atualização.");

        var outro = await _tutorRepository.ObterPorCpfAsync(dados.Cpf);
        if (outro is not null && outro.Id != id)
            throw new InvalidOperationException("Já existe um tutor cadastrado com este CPF.");

        dados.Id = id;
        dados.Pets.Clear();
        var atualizado = await _tutorRepository.AtualizarAsync(dados);
        await AuditarAsync("Tutor", "Atualizacao", atualizado.Nome);
        return atualizado;
    }

    public async Task ExcluirAsync(int id)
    {
        var tutor = await _tutorRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Tutor não encontrado para exclusão.");

        if (await _tutorRepository.PossuiPetsAsync(id))
            throw new ConflictException("Não é possível excluir este tutor, pois ele possui pets vinculados.");

        await _tutorRepository.RemoverAsync(tutor);
        await AuditarAsync("Tutor", "Exclusao", tutor.Nome);
    }
}
