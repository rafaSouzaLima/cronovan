using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Services;

namespace webapp.Areas.Gerente.Pages.Estudantes;

[Authorize(Roles = "Gerente")]
class DeleteModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly EstudanteService _estudanteService;

    public DeleteModel(ILogger<CreateModel> logger, EstudanteService estudanteService) {
        _logger = logger;
        _estudanteService = estudanteService;
    }

    public async Task<IActionResult> OnPostAsync(string cpf) {
        var result = await _estudanteService.ExcluirEstudanteAsync(cpf);
        
        if(!result.Succeeded && result.Errors.Any(e => e.Code == "EstudanteNotFound")) {
            return NotFound();
        }
        
        return RedirectToPage("/Estudantes/Index", new { area = "Gerente"});
    }
}