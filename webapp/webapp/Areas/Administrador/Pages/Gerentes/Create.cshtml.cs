using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Models;

namespace webapp.Areas.Administrador.Pages.Gerentes;

[Authorize(Roles = "Administrador")]
class CreateModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly UserManager<Usuario> _userManager;

    public CreateModel(ILogger<CreateModel> logger, UserManager<Usuario> userManager) {
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult OnGet() {
        return Page();
    }

    public class InputModel {
        public string Nome { get; set; }
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmarSenha { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnPostAsync() {
        if(!ModelState.IsValid) return Page();

        var gerente = new Models.Gerente {
            Nome = Input.Nome,
            Email = Input.Email,
            UserName = Input.Email,
            NormalizedEmail = Input.Email.ToUpper(),
            NormalizedUserName = Input.Email.ToUpper()
        };

        var result = await _userManager.CreateAsync(gerente, Input.Senha);

        if(!result.Succeeded) {
            foreach (var error in result.Errors) {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        
        await _userManager.AddToRoleAsync(gerente, "Gerente");
        return RedirectToPage("/Gerentes/Index", new { area = "Administrador"});
    }

}