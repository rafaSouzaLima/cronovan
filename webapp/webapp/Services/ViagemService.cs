using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapp.Areas.Gerente.Pages.Viagens;
using webapp.Data;
using webapp.Models;

namespace webapp.Services;

public class ViagemService {
    private readonly CronovanContext _context;

    public ViagemService(CronovanContext context) {
        _context = context;
    }

    public async Task<string?> CriarViagemAsync(Viagem viagem, string motoristaCnh, string numeroRenavam) {
        if (viagem == null)
            throw new ArgumentNullException(nameof(viagem));
        
        var motorista = await _context.Motoristas.FirstOrDefaultAsync(m => m.Cnh == motoristaCnh);

        if(motorista == null)
            throw new ArgumentException($"O motorista com a CNH '{motoristaCnh}' não existe!");

        if(!await _context.Veiculos.AnyAsync(v => v.NumeroRenavam == numeroRenavam))
            throw new ArgumentException($"O veículo com o número de RENAVAM '{numeroRenavam}' não existe!");
        
        viagem.MotoristaId = motorista.Id;
        viagem.NumeroRenavam = numeroRenavam;

        if (await _context.Viagens.AnyAsync(v => viagem.Chegada >= v.Saida && 
                       viagem.Saida <= v.Chegada && 
                       v.MotoristaId == viagem.MotoristaId))
            return $"O motorista {motorista.Nome} já está em uma viagem no mesmo período!";

        if (await _context.Viagens.AnyAsync(v => viagem.Chegada >= v.Saida && 
                       viagem.Saida <= v.Chegada && 
                       v.NumeroRenavam == viagem.NumeroRenavam))
            return $"O veículo com o número de RENAVAM {numeroRenavam} já foi selecionado no mesmo período!";

        _context.Viagens.Add(viagem);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<string?> EditarViagemAsync(int id, Viagem viagemEditar, string motoristaCnh, string numeroRenavam, IList<InputAgendamentoForm> agendamentoForms) {
        var viagem = await _context.Viagens
            .Include(v => v.Estudantes)
            .FirstOrDefaultAsync(v => v.Id == id);
        if (viagem == null)
            throw new ArgumentNullException(nameof(viagem));

        var motorista = await _context.Motoristas.FirstOrDefaultAsync(m => m.Cnh == motoristaCnh);

        if(motorista == null)
            throw new ArgumentException($"O motorista com a CNH '{motoristaCnh}' não existe!");

        if(!await _context.Veiculos.AnyAsync(v => v.NumeroRenavam == numeroRenavam))
            throw new ArgumentException($"O veículo com o número de RENAVAM '{numeroRenavam}' não existe!");

        viagem.MotoristaId = motorista.Id;
        viagem.NumeroRenavam = numeroRenavam;
        viagem.Chegada = viagemEditar.Chegada;
        viagem.Saida = viagemEditar.Saida;

        viagem.Agendamentos.Clear();
        foreach(var iaf in agendamentoForms) {
            var estudante = await _context.Estudantes.FirstOrDefaultAsync(e => e.Cpf == iaf.Cpf);
            if(estudante != null) {
                viagem.Agendamentos.Add(
                    new Agendamento {
                        Estudante = estudante,
                        Chegada = iaf.DataHora
                    }
                );
            }
        }

        await _context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> ExcluirViagemAsync(int id) {
        var viagem = await _context.Viagens.FirstOrDefaultAsync(v => v.Id == id);

        if(viagem == null)
            return "Viagem não encontrada";

        _context.Viagens.Remove(viagem);
        await _context.SaveChangesAsync();

        return null;
    }
}