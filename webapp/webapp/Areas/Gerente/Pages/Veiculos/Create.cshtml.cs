using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Gerente.Pages.Veiculos;

[Authorize(Roles = "Gerente")]
class CreateModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly CronovanContext _context;

    public CreateModel(ILogger<CreateModel> logger, CronovanContext context) {
        _logger = logger;
        _context = context;
    }

    public class InputModel {
        public required string NumeroRenavam { get; set; }
        public required int NumeroLugares { get; set; }
        public required string? Descricao { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IList<Models.Veiculo> Veiculos { get; set; } = new List<Models.Veiculo>(); 

    public async Task<IActionResult> OnGetAsync() {
        if(_context.Veiculos != null)
            Veiculos = await _context.Veiculos.ToListAsync();

        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync() {
        if(!ModelState.IsValid) return Page();

        var veiculo = new Models.Veiculo {
            NumeroRenavam = Input.NumeroRenavam,
            NumeroLugares = Input.NumeroLugares,
            Descricao = Input.Descricao
        };

        _context.Veiculos.Add(veiculo);
        await _context.SaveChangesAsync();

        // if(!result.Succeeded) {
        //     foreach (var error in result.Errors) {
        //         ModelState.AddModelError(string.Empty, error.Description);
        //     }
        //     return Page();
        // }

        return RedirectToPage("/Veiculos/Index", new { area = "Gerente"});
    }

}