using System.ComponentModel.DataAnnotations;

namespace webapp.Models;

public class Veiculo {
    [Key]
    public required string NumeroRenavam { get; set; }
    
    public required int NumeroLugares { get; set; }
    public string? Descricao { get; set; }
}