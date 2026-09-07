using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class TutorService
{
    private readonly ITutorRepository _tutorRepository;

    public TutorService(ITutorRepository tutorRepository)
    {
        _tutorRepository = tutorRepository;
    }

    public async Task<Tutor> CadastrarTutorAsync(Tutor tutor)
    {
        if (string.IsNullOrWhiteSpace(tutor.Nome) || string.IsNullOrWhiteSpace(tutor.Cpf))
        {
            throw new ArgumentException("O Nome e o CPF do Tutor são obrigatórios.");
        }

        var cpfExiste = await _tutorRepository.ExisteCpfAsync(tutor.Cpf);
        if (cpfExiste)
        {
            throw new InvalidOperationException("Já existe um tutor cadastrado com este CPF.");
        }

        return await _tutorRepository.AdicionarAsync(tutor);
    }
}