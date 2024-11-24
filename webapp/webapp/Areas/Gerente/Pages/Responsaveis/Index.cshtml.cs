using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Responsaveis;

[Authorize(Roles = "Gerente")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public IList<Models.Responsavel> Responsaveis { get; set; } = new List<Models.Responsavel>();

    public async Task OnGetAsync() {
        if(_context.Motoristas != null) {
            Responsaveis = await _context.Responsaveis
                                    .Include(m => m.Endereco)
                                    .ToListAsync();
        }
    }
}