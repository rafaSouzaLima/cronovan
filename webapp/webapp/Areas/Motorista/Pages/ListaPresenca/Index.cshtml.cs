using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Motorista.Pages.ListaPresenca;

[Authorize(Roles = "Motorista")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<Usuario> _userManager;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, UserManager<Usuario> userManager, CronovanContext context) {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public IList<Models.Viagem> Viagens { get; set; } = [];

    public async Task LoadDataAsync() {
        var Usuario = await _userManager.GetUserAsync(User);

        if(_context.Viagens != null) {
            Viagens = await _context.Viagens
                                        .OrderBy(v => v.Saida)
                                        .Where(v => v.MotoristaId == Usuario!.Id && v.Saida >= DateTime.Now)
                                        .ToListAsync();
        }
    }

    public async Task<IActionResult> OnGetAsync() {
        await LoadDataAsync();
        return Page();
    }
}