using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using webapp.Validators;

namespace webapp.Models;

[IntervalValidation(nameof(Saida), nameof(Chegada))]
public class Viagem {
    public int Id { get; set; }

    [DisplayName("Data de Saída")]
    public required DateTime Saida { get; set; }
    
    [DisplayName("Data de Chegada")]
    public required DateTime Chegada { get; set; }

    [DisplayName("Número RENAVAM")]
    public string NumeroRenavam { get; set; }
    [DisplayName("Veículo")]
    public Veiculo Veiculo { get; set; }
        
    public string MotoristaId { get; set; }
    [DisplayName("Motorista")]
    public Motorista Motorista { get; set; }

    public ICollection<Estudante> Estudantes { get; } = [];
    public ICollection<Agendamento> Agendamentos { get; } = [];
}