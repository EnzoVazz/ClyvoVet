using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class ProntuarioService
{
    private readonly IProntuarioRepository _prontuarioRepository;

    public ProntuarioService(IProntuarioRepository prontuarioRepository)
    {
        _prontuarioRepository = prontuarioRepository;
    }

    public async Task<Prontuario> RegistrarProntuarioAsync(Prontuario prontuario)
    {
        // Validação 1: O diagnóstico não pode ser vazio
        if (string.IsNullOrWhiteSpace(prontuario.Diagnostico))
        {
            throw new ArgumentException("O Diagnóstico é obrigatório para registrar um prontuário.");
        }

        // Regra de Negócio: Se a data não foi informada, assume o momento exato do registro
        if (!prontuario.DataRegistro.HasValue)
        {
            prontuario.DataRegistro = DateTime.Now;
        }

        return await _prontuarioRepository.AdicionarAsync(prontuario);
    }
}