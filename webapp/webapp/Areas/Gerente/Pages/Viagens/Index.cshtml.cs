using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Viagens;

[Authorize(Roles = "Gerente")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public IList<Models.Viagem> Viagens { get; set; } = new List<Models.Viagem>();

    public async Task OnGetAsync() {
        if(_context.Viagens != null)
            Viagens = await _context.Viagens
                                .Include(v => v.Motorista)
                                .Include(v => v.Veiculo)
                                .ToListAsync();
    }
}