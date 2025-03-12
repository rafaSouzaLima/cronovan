using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Models;

namespace webapp.Pages;

[Authorize]
public class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IndexModel(ILogger<IndexModel> logger, RoleManager<IdentityRole> roleManager) {
        _logger = logger;
        _roleManager = roleManager;
    }

    public IActionResult OnGet() {
        string returnUrl = string.Empty;

        if(User.IsInRole("Administrador")) {
            returnUrl = Url.Page("/Gerentes/Index", new { area = "Administrador" })!;
        } else if(User.IsInRole("Gerente")) {
            returnUrl = Url.Page("Estudantes/Index", new { area = "Gerente"})!;
        } else if(User.IsInRole("Estudante")) {
            returnUrl = Url.Page("/Cronograma/Index", new { area = "Estudante"})!;
        } else if(User.IsInRole("Responsavel")) {
            returnUrl = Url.Page("/Cronogramas/Index", new { area = "Responsavel"})!;
        } else if(User.IsInRole("Motorista")) {
            returnUrl = Url.Page("/ListaPresenca/Index", new { area = "Motorista"})!;
        }

        return LocalRedirect(returnUrl);
    }
}
