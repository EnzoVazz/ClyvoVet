using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class VeterinarioService
{
    private readonly IVeterinarioRepository _veterinarioRepository;

    public VeterinarioService(IVeterinarioRepository veterinarioRepository)
    {
        _veterinarioRepository = veterinarioRepository;
    }

    public async Task<Veterinario> CadastrarVeterinarioAsync(Veterinario veterinario)
    {
        if (string.IsNullOrWhiteSpace(veterinario.Nome) || string.IsNullOrWhiteSpace(veterinario.Crmv))
        {
            throw new ArgumentException("O Nome e o CRMV do Veterinário são obrigatórios.");
        }

        var crmvExiste = await _veterinarioRepository.ExisteCrmvAsync(veterinario.Crmv);
        if (crmvExiste)
        {
            throw new InvalidOperationException("Já existe um veterinário cadastrado com este CRMV.");
        }

        return await _veterinarioRepository.AdicionarAsync(veterinario);
    }
}