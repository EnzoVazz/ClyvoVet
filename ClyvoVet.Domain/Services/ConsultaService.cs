using ClyvoVet.Domain.Entities;
using ClyvoVet.Domain.Interfaces;

namespace ClyvoVet.Domain.Services;

public class ConsultaService
{
    private readonly IConsultaRepository _consultaRepository;

    public ConsultaService(IConsultaRepository consultaRepository)
    {
        _consultaRepository = consultaRepository;
    }

    public async Task<Consulta> AgendarConsultaAsync(Consulta consulta)
    {
        if (consulta.IdVeterinario <= 0 || consulta.IdPet <= 0 || consulta.IdClinica <= 0)
        {
            throw new ArgumentException("Os IDs do Veterinário, Pet e Clínica são obrigatórios para agendar uma consulta.");
        }

        if (string.IsNullOrWhiteSpace(consulta.Status))
        {
            consulta.Status = "Agendada";
        }

        return await _consultaRepository.AdicionarAsync(consulta);
    }
}