using System.ComponentModel;

namespace webapp.Models;

public class Agendamento {
    [DisplayName("Viagem")]
    public Viagem Viagem { get; set; }
    public int ViagemId { get; set; }

    [DisplayName("Estudante")]
    public Estudante Estudante { get; set; }
    public string EstudanteId { get; set; }
    
    public DateTime Chegada { get; set; }
    public bool PresencaEstudante { get; set; } = false;
    public bool PresencaConfirmada { get; set; } = false;
}