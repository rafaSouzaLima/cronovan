using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Estudantes;

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

    public class InputModel {

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

    public class InputResponsavelModel {

        public string? Cpf { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmarSenha { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
        public string? Telefone { get; set; }
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [BindProperty]
    public InputResponsavelModel InputResponsavel { get; set; }

    [BindProperty]
    public string? CpfResponsavel { get; set; }

    public IList<Models.Responsavel> Responsaveis { get; set; } = new List<Models.Responsavel>(); 

    public async Task<IActionResult> OnGetAsync() {
        if(_context.Responsaveis != null)
            Responsaveis = await _context.Responsaveis
                                            .Include(r => r.Endereco)
                                            .ToListAsync();

        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync() {
        if(!ModelState.IsValid) return Page();

        var estudante = new Models.Estudante {
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

        if(estudante.IsUnderage()) {
            Models.Responsavel? responsavel;

            if(CpfResponsavel != null) {
                responsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => r.Cpf == CpfResponsavel);

                if(responsavel == null) {
                    ModelState.AddModelError(string.Empty, "Responsável com CPF informado não existe. Tente criá-lo!");
                    return Page();
                }
            } else {
                responsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => r.Cpf == InputResponsavel.Cpf);

                if(responsavel != null) {
                    ModelState.AddModelError(string.Empty, "Responsável com CPF informado já existe. Tente associá-lo!");
                    return Page();
                }

                //Se não existe crie-o
                responsavel = new Models.Responsavel {
                    Nome = InputResponsavel.Nome,
                    Cpf = InputResponsavel.Cpf,
                    Email = InputResponsavel.Email,
                    UserName = InputResponsavel.Email,
                    NormalizedEmail = InputResponsavel.Email.ToUpper(),
                    NormalizedUserName = InputResponsavel.Email.ToUpper(),
                    DataNascimento = InputResponsavel.DataNascimento ?? DateTime.MinValue,
                    Telefone = InputResponsavel.Telefone
                };
                
                var enderecoResponsavel = new Endereco {
                    Rua = InputResponsavel.Rua ?? string.Empty,
                    Numero = InputResponsavel.Numero ?? string.Empty,
                    Bairro = InputResponsavel.Bairro ?? string.Empty,
                    Cidade = InputResponsavel.Cidade ?? string.Empty,
                    Estado = InputResponsavel.Estado ?? string.Empty
                };

                var resultResponsavel = await _userManager.CreateAsync(responsavel, InputResponsavel.Senha ?? string.Empty);

                if(!resultResponsavel.Succeeded) {
                    foreach (var error in resultResponsavel.Errors) {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                enderecoResponsavel.UsuarioId = responsavel.Id;
                _context.Enderecos.Add(enderecoResponsavel);
                await _context.SaveChangesAsync();

                await _userManager.AddToRoleAsync(responsavel, "Responsavel");
            }
            
            estudante.ResponsavelId = responsavel.Id;
        }

        var result = await _userManager.CreateAsync(estudante, Input.Senha);

        if(!result.Succeeded) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        
        endereco.UsuarioId = estudante.Id;
        _context.Enderecos.Add(endereco);
        await _context.SaveChangesAsync();

        await _userManager.AddToRoleAsync(estudante, "Estudante");
        return RedirectToPage("/Estudantes/Index", new { area = "Gerente"});
    }

}