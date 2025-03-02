using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;
using webapp.Services;
using webapp.Validators;

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

    [IntervalValidation(nameof(Saida), nameof(Chegada), ErrorMessage = "A data de saída deve ser inferior a data de chegada!")]
    public class InputModel {
        [Required(ErrorMessage = "A CNH é obrigatória")]
        public required string MotoristaCnh { get; set; }

        [Required(ErrorMessage = "O número do RENAVAM é obrigatório")]
        public required string NumeroRenavam { get; set; }

        [Required(ErrorMessage = "A data de saída é obrigatória")]
        [FutureDate(ErrorMessage = "A data de saída deve estar no futuro")]
        public DateTime? Saida { get; set; }
        
        [Required(ErrorMessage = "A data de chegada é obrigatória")]
        [FutureDate(ErrorMessage = "A data de saída deve estar no futuro")]
        public DateTime? Chegada { get; set; }
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
        if(!ModelState.IsValid) {
            await LoadDataAsync();
            return Page();
        } 

        var viagem = new Models.Viagem {
            Saida = Input.Saida ?? DateTime.Now,
            Chegada = Input.Chegada ?? DateTime.Now.AddHours(2)
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