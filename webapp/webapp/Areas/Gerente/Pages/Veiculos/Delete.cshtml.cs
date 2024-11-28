using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;

namespace webapp.Areas.Gerente.Pages.Veiculos;

[Authorize(Roles = "Gerente")]
class DeleteModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly CronovanContext _context;

    public DeleteModel(ILogger<CreateModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync(string renavam) {
        var veiculo = await _context.Veiculos.FirstOrDefaultAsync(v => v.NumeroRenavam == renavam);

        if(veiculo == null) {
            return NotFound();
        }

        _context.Veiculos.Remove(veiculo);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Veiculos/Index", new { area = "Gerente"});
    }
}