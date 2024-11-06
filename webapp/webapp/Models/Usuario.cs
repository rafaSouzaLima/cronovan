using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace webapp.Models;

public class Usuario : IdentityUser {
    public required string Nome { get; set; }
    public required string Cpf { get; set; }
    public required string Telefone { get; set; }

    [ForeignKey("User")]
    public required Endereco Endereco { get; set; }

    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; }
    public bool Ativo { get; set; } = false;
}