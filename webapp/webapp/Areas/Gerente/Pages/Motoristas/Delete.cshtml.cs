using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;

namespace webapp.Areas.Gerente.Pages.Motoristas;

[Authorize(Roles = "Gerente")]
class DeleteModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly CronovanContext _context;

    public DeleteModel(ILogger<CreateModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> OnPostAsync(string cnh) {
        var motorista = await _context.Motoristas.FirstOrDefaultAsync(m => m.Cnh == cnh);

        if(motorista == null) {
            return NotFound();
        }

        _context.Motoristas.Remove(motorista);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Motoristas/Index", new { area = "Gerente"});
    }
}