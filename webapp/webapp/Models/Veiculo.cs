using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Veiculo {
    [Key]
    [DisplayName("Número RENAVAM")]
    public required string NumeroRenavam { get; set; }
    
    [DisplayName("Número de Lugares")]
    public required int NumeroLugares { get; set; }

    [DisplayName("Descrição")]
    public string? Descricao { get; set; }

    public ICollection<Viagem> Viagens { get; } = [];
}