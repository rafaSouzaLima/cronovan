using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;
using webapp.Services;

namespace webapp.Areas.Gerente.Pages.Viagens;

[Authorize(Roles = "Gerente")]
class CreateModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly ViagemService _viagemService;
    private readonly CronovanContext _context;

    public CreateModel(ILogger<CreateModel> logger, ViagemService viagemService ,CronovanContext context) {
        _logger = logger;
        _viagemService = viagemService;
        _context = context;
    }

    public class InputModel {
        public required string MotoristaCnh { get; set; }
        public required string NumeroRenavam { get; set; }
        public required DateTime Saida { get; set; }
        public required DateTime Chegada { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IList<Models.Veiculo> Veiculos { get; set; } = new List<Models.Veiculo>(); 
    public IList<Models.Motorista> Motoristas { get; set; } = new List<Models.Motorista>(); 

    public async Task LoadDataAsync() {
        if(_context.Veiculos != null && _context.Motoristas != null) {
            Veiculos = await _context.Veiculos.ToListAsync();
            Motoristas = await _context.Motoristas.Include(m => m.Endereco)
                                                  .ToListAsync();
        }
    }

    public async Task<IActionResult> OnGetAsync() {
        await LoadDataAsync();
        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync() {
        if(!ModelState.IsValid) return Page();

        var viagem = new Models.Viagem {
            Saida = Input.Saida,
            Chegada = Input.Chegada
        };

        var result = await _viagemService.CriarViagemAsync(viagem, Input.MotoristaCnh, Input.NumeroRenavam);

        if(result != null) {
            ModelState.AddModelError(string.Empty, result);
            
            await LoadDataAsync();
            return Page();
        }

        return RedirectToPage("/Viagens/Index", new { area = "Gerente"});
    }

}