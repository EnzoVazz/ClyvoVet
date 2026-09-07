using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class ClinicaService
{
    private readonly IClinicaRepository _clinicaRepository;

    public ClinicaService(IClinicaRepository clinicaRepository)
    {
        _clinicaRepository = clinicaRepository;
    }

    public async Task<Clinica> CadastrarClinicaAsync(Clinica clinica)
    {
        if (string.IsNullOrWhiteSpace(clinica.Nome) || string.IsNullOrWhiteSpace(clinica.Cnpj))
        {
            throw new ArgumentException("O Nome e o CNPJ da Clínica são obrigatórios.");
        }

        var cnpjExiste = await _clinicaRepository.ExisteCnpjAsync(clinica.Cnpj);
        if (cnpjExiste)
        {
            throw new InvalidOperationException("Já existe uma clínica cadastrada com este CNPJ.");
        }

        return await _clinicaRepository.AdicionarAsync(clinica);
    }
}