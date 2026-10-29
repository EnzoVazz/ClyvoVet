using ClyvoVet.Domain.Common;
using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Exceptions;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class PetService : AppService
{
    private readonly IPetRepository _petRepository;

    public PetService(IPetRepository petRepository, IAuditoriaRepository auditoria, IUsuarioAtual usuarioAtual)
        : base(auditoria, usuarioAtual)
    {
        _petRepository = petRepository;
    }

    public Task<PagedResult<Pet>> ListarAsync(PageQuery query, string? especie, int? idTutor)
        => _petRepository.ListarAsync(query, especie, idTutor);

    public async Task<Pet> ObterPorIdAsync(int id)
    {
        return await _petRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Pet não encontrado.");
    }

    public async Task<Pet> CadastrarPetAsync(Pet pet)
    {
        if (string.IsNullOrWhiteSpace(pet.Nome) || string.IsNullOrWhiteSpace(pet.Especie))
            throw new ArgumentException("O Nome e a Espécie do Pet são obrigatórios.");

        if (pet.IdTutor <= 0)
            throw new ArgumentException("O ID do Tutor é obrigatório.");

        pet.Tutor = null;
        pet.Consultas.Clear();

        var salvo = await _petRepository.AdicionarAsync(pet);
        await AuditarAsync("Pet", "Cadastro", salvo.Nome);
        return salvo;
    }

    public async Task<Pet> AtualizarAsync(int id, Pet dados)
    {
        GarantirMesmoId(id, dados.Id);

        if (string.IsNullOrWhiteSpace(dados.Nome) || string.IsNullOrWhiteSpace(dados.Especie))
            throw new ArgumentException("O Nome e a Espécie do Pet são obrigatórios.");

        if (dados.IdTutor <= 0)
            throw new ArgumentException("O ID do Tutor é obrigatório.");

        if (await _petRepository.ObterPorIdAsync(id) is null)
            throw new NotFoundException("Pet não encontrado para atualização.");

        dados.Id = id;
        dados.Tutor = null;
        var atualizado = await _petRepository.AtualizarAsync(dados);
        await AuditarAsync("Pet", "Atualizacao", atualizado.Nome);
        return atualizado;
    }

    public async Task ExcluirAsync(int id)
    {
        var pet = await _petRepository.ObterPorIdAsync(id)
            ?? throw new NotFoundException("Pet não encontrado para exclusão.");

        if (await _petRepository.PossuiConsultasAsync(id))
            throw new ConflictException("Não é possível excluir este pet, pois ele possui consultas vinculadas no histórico.");

        await _petRepository.RemoverAsync(pet);
        await AuditarAsync("Pet", "Exclusao", pet.Nome);
    }
}
