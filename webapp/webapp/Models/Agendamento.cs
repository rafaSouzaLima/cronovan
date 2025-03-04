using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Agendamento {
    [DisplayName("Viagem")]
    public Viagem Viagem { get; set; }
    public int ViagemId { get; set; }

    [DisplayName("Estudante")]
    public Estudante Estudante { get; set; }
    public string EstudanteId { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Chegada { get; set; }
    public bool PresencaEstudante { get; set; } = false;
    public bool PresencaConfirmada { get; set; } = false;
}