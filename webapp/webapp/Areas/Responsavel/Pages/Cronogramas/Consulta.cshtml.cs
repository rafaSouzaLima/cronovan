using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webapp.Data;
using webapp.Models;
using webapp.Pagination;
using webapp.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace webapp.Areas.Responsavel.Pages.Cronogramas;

[Authorize(Roles = "Responsavel")]
class ConsultaModel : PageModel {
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<Usuario> _userManager;
    private readonly CronovanContext _context;

    public ConsultaModel(ILogger<IndexModel> logger, UserManager<Usuario> userManager, CronovanContext context) {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }

    public IList<DateTime> DiasDaSemana { get; set; } = [];
    public Models.Estudante Estudante { get; set; }
    public WeeklyPagination<Agendamento> Agendamentos { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string Cpf { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime Data { get; set; } = DateTime.Today;
    [BindProperty]
    public IList<InputModel> Presencas { get; set; } = [];

    public record InputModel {
        public required bool PresencaEstudante { get; set; }
        public required int ViagemId { get; set; }
    };

    public async Task LoadDataAsync() {
        var Usuario = await _userManager.GetUserAsync(User);

        if(_context.Agendamentos == null) return;

        await Agendamentos.PaginateAsync(
            _context.Agendamentos
                .Include(a => a.Viagem)
                .Include(a => a.Estudante)
                .Where(a => a.EstudanteId == Estudante.Id),
            Data,
            a => a.Chegada
        );
        DiasDaSemana = GetDiasDaSemana();
    }

    public async Task<IActionResult> OnGetAsync() {
        var usuario = await _userManager.GetUserAsync(User);

        if(usuario is null) return Unauthorized();

        Cpf = Regex.Replace(Cpf, @"\D", "");
        var estudante = await _context.Estudantes.FirstOrDefaultAsync(e => e.Cpf == Cpf);

        if(estudante is null) return NotFound();
        Estudante = estudante!;
        if(Estudante.ResponsavelId != usuario.Id) return Forbid();

        await LoadDataAsync();

        foreach (var agendamento in Agendamentos)
            Presencas.Add(new InputModel {
                PresencaEstudante = agendamento.PresencaEstudante,
                ViagemId = agendamento.Viagem.Id
            });

        return Page();
    }

    public async Task<IActionResult> OnPostAsync() {
        if(!ModelState.IsValid) {
            await LoadDataAsync();
            return Page();
        }
        
        var Usuario = await _userManager.GetUserAsync(User);
        if(Usuario == null) return Unauthorized();

        var agendamentos = await _context.Agendamentos.Include(a => a.Estudante)
                                                    .Where(a => a.Estudante.Cpf == Cpf)
                                                    .ToListAsync();
        foreach(var agendamento in agendamentos) {
            var presenca = Presencas.FirstOrDefault(p => p.ViagemId == agendamento.ViagemId);
            if (presenca is not null)
                agendamento.PresencaEstudante = presenca.PresencaEstudante;
        }
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    private List<DateTime> GetDiasDaSemana() {
        var primeiroDia = Data.StartOfWeek(DayOfWeek.Sunday);

        return Enumerable.Range(0, 7)
                        .Select(i => primeiroDia.AddDays(i))
                        .ToList();
    }
}