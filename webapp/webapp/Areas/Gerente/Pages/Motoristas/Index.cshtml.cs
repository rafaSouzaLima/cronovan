using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Motoristas;

[Authorize(Roles = "Gerente")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public IList<Models.Motorista> Motoristas { get; set; } = new List<Models.Motorista>();

    public async Task OnGetAsync() {
        if(_context.Motoristas != null) {
            Motoristas = await _context.Motoristas
                                    .Include(m => m.Endereco)
                                    .ToListAsync();
        }
    }
}