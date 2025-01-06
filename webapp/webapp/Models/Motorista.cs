using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace webapp.Models;

[Index(nameof(Cnh), IsUnique = true)]
public class Motorista : Usuario {
    [DisplayName("CNH")]
    public required string Cnh { get; set; }

    public ICollection<Viagem> Viagens { get; } = [];
}