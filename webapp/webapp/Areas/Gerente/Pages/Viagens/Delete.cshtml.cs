using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Services;

namespace webapp.Areas.Gerente.Pages.Viagens;

[Authorize(Roles = "Gerente")]
class DeleteModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly ViagemService _viagemService;

    public DeleteModel(ILogger<CreateModel> logger, ViagemService viagemService) {
        _logger = logger;
        _viagemService = viagemService;
    }

    public async Task<IActionResult> OnPostAsync(int id) {
        var result = await _viagemService.ExcluirViagemAsync(id);

        if(result != null) {
            return NotFound();
        }

        return RedirectToPage("/Viagens/Index", new { area = "Gerente"});
    }
}