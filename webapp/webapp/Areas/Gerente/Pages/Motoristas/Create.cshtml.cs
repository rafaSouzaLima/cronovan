using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Motoristas;

[Authorize(Roles = "Gerente")]
class CreateModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly UserManager<Usuario> _userManager;
    private readonly CronovanContext _context;

    public CreateModel(ILogger<CreateModel> logger, UserManager<Usuario> userManager, CronovanContext context) {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public IActionResult OnGet() {
        return Page();
    }

    public class InputModel {
        public required string Cnh { get; set; }
        public required string Cpf { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Senha { get; set; }

        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmarSenha { get; set; }
        
        [DataType(DataType.Date)]
        public required DateTime DataNascimento { get; set; }
        public required string Telefone { get; set; }
        public required string Rua { get; set; }
        public required string Numero { get; set; }
        public required string Bairro { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnPostAsync() {
        if(!ModelState.IsValid) return Page();

        var motorista = new Models.Motorista {
            Cnh = Input.Cnh,
            Cpf = Input.Cpf,
            Nome = Input.Nome,
            Email = Input.Email,
            UserName = Input.Email,
            NormalizedEmail = Input.Email.ToUpper(),
            NormalizedUserName = Input.Email.ToUpper(),
            DataNascimento = Input.DataNascimento,
            Telefone = Input.Telefone
        };

        var endereco = new Endereco {
            Rua = Input.Rua,
            Numero = Input.Numero,
            Bairro = Input.Bairro,
            Cidade = Input.Cidade,
            Estado = Input.Estado
        };

        var result = await _userManager.CreateAsync(motorista, Input.Senha);

        if(!result.Succeeded) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        
        endereco.UsuarioId = motorista.Id;
        _context.Enderecos.Add(endereco);
        await _context.SaveChangesAsync();

        await _userManager.AddToRoleAsync(motorista, "Motorista");
        return RedirectToPage("/Motoristas/Index", new { area = "Gerente"});
    }

}