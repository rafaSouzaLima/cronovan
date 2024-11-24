using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Administrador.Pages.Gerentes;

[Authorize(Roles = "Administrador")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public IList<Models.Gerente> Gerentes { get; set; } = default!;

    public async Task OnGetAsync() {
        if(_context.Usuarios != null) {
            Gerentes = await _context.Gerentes.ToListAsync();
        }
    }
}