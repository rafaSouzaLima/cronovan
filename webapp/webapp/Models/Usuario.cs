using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace webapp.Models;

public class Usuario : IdentityUser {
    public const int IDADE_MAIORIDADE = 18;

    [DisplayName("E-mail")]
    public override string? Email { get => base.Email; set => base.Email = value; }
    
    [DisplayName("Nome de Usuário")]
    public override string? UserName { get => base.UserName; set => base.UserName = value; }

    [DisplayName("Nome")]
    public string? Nome { get; set; }

    [DisplayName("CPF")]
    public string? Cpf { get; set; }

    [DisplayName("Telefone")]
    public string? Telefone { get; set; }

    [DisplayName("Endereço")]
    public Endereco? Endereco { get; set; }

    [DisplayName("Data de Nascimento")]
    [DataType(DataType.Date)]
    public DateTime DataNascimento { get; set; }

    public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; }

    public bool IsUnderage() {
        var hoje = DateTime.Today;
        int idade = hoje.Year - DataNascimento.Year;

        if(DataNascimento.DayOfYear > hoje.DayOfYear) idade--;

        return idade < IDADE_MAIORIDADE;
    }
}