using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Services;

namespace webapp.Areas.Gerente.Pages.Motoristas;

[Authorize(Roles = "Gerente")]
class DeleteModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly MotoristaService _motoristaService;

    public DeleteModel(ILogger<CreateModel> logger, MotoristaService motoristaService) {
        _logger = logger;
        _motoristaService = motoristaService;
    }

    public async Task<IActionResult> OnPostAsync(string cnh) {
        var result = await _motoristaService.ExcluirMotoristaAsync(cnh);
        
        if(!result.Succeeded && result.Errors.Any(e => e.Code == "MotoristaNotFound")) {
            return NotFound();
        }
        
        return RedirectToPage("/Motoristas/Index", new { area = "Gerente"});
    }
}