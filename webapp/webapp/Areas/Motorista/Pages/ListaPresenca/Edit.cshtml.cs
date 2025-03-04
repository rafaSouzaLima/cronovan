using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Areas.Motorista.Pages.ListaPresenca;

[Authorize(Roles = "Motorista")]
class EditModel : PageModel {
    private readonly ILogger<EditModel> _logger;
    private readonly CronovanContext _context;
    private readonly UserManager<Usuario> _userManager;

    public EditModel(ILogger<EditModel> logger, CronovanContext context, UserManager<Usuario> userManager) {
        _logger = logger;
        _context = context;
        _userManager = userManager;
    }

    public Viagem? Viagem { get; set; }

    [BindProperty]
    public IList<InputModel> Confirmacoes { get; init; } = [];

    public record InputModel {
        public required bool PresencaConfirmada { get; set; }
        public required string Cpf { get; set; }
    };

    public async Task LoadDataAsync(int id) {
        var Usuario = await _userManager.GetUserAsync(User);

        if (_context.Viagens == null) return;

        Viagem = await _context.Viagens.Include(v => v.Motorista)
                                    .Include(v => v.Veiculo)
                                    .Include(v => v.Agendamentos
                                        .Where(a => a.PresencaEstudante)
                                        .OrderBy(a => a.Chegada))
                                        .ThenInclude(a => a.Estudante)
                                            .ThenInclude(e => e.Endereco)
                                    .FirstOrDefaultAsync(v => v.Id == id && v.MotoristaId == Usuario!.Id);
    }

    public async Task<IActionResult> OnGetAsync(int id) {
        await LoadDataAsync(id);
        if(Viagem == null) return NotFound();

        foreach (var agendamento in Viagem.Agendamentos)
            Confirmacoes.Add(new InputModel {
                PresencaConfirmada = agendamento.PresencaConfirmada,
                Cpf = agendamento.Estudante?.Cpf ?? string.Empty
            });

        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync(int id) {
        if(!ModelState.IsValid) {
            await LoadDataAsync(id);
            return Page();
        }

        var agendamentos = await _context.Agendamentos
            .Include(a => a.Estudante)
            .Where(a => a.ViagemId == id)
            .ToListAsync();

        foreach (var agendamento in agendamentos) {
            var confirmacao = Confirmacoes.FirstOrDefault(c => c.Cpf == agendamento.Estudante.Cpf);
            if (confirmacao != null) {
                agendamento.PresencaConfirmada = confirmacao.PresencaConfirmada;
            }
        }
        await _context.SaveChangesAsync();

        return RedirectToPage("/ListaPresenca/Index", new { area = "Motorista"});
    }

}