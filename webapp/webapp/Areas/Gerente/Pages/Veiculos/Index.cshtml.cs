using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Veiculos;

[Authorize(Roles = "Gerente")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public IList<Models.Veiculo> Veiculos { get; set; } = new List<Models.Veiculo>();

    public async Task OnGetAsync() {
        if(_context.Veiculos != null)
            Veiculos = await _context.Veiculos.ToListAsync();
    }
}