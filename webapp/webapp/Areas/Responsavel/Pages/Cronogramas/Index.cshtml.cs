using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Responsavel.Pages.Cronogramas;

[Authorize(Roles = "Responsavel")]
class IndexModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<Usuario> _userManager;
    private readonly CronovanContext _context;

    public IndexModel(ILogger<IndexModel> logger, UserManager<Usuario> userManager, CronovanContext context) {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public IList<Models.Estudante> Estudantes { get; set; } = [];

    public async Task LoadDataAsync() {
        var Usuario = await _userManager.GetUserAsync(User);

        if(_context.Estudantes != null) {
            Estudantes = await _context.Estudantes.Where(e => e.ResponsavelId == Usuario!.Id)
                                            .OrderBy(e => e.Nome)
                                            .ToListAsync();
        }
    }

    public async Task<IActionResult> OnGetAsync() {
        await LoadDataAsync();
        return Page();
    }
}