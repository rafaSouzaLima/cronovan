using System.ComponentModel;

namespace webapp.Models;

public class Motorista : Usuario {
    [DisplayName("CNH")]
    public required string Cnh { get; set; }
}