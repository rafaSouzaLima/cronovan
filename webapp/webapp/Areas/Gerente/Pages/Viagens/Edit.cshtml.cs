using System.ComponentModel;
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
class EditModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly ViagemService _viagemService;
    private readonly CronovanContext _context;

    public EditModel(ILogger<CreateModel> logger, ViagemService viagemService, CronovanContext context) {
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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Saida { get; set; }
        
        [Required(ErrorMessage = "A data de chegada é obrigatória")]
        [FutureDate(ErrorMessage = "A data de saída deve estar no futuro")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? Chegada { get; set; }
    }

    public int Id { get; set; }

    [BindProperty]
    public InputModel Input { get; set; }

    [BindProperty]
    public IList<InputAgendamentoForm> InputAgendamentoForms { get; set; } = [];

    public Viagem? Viagem { get; set; }

    public IList<Models.Veiculo> Veiculos { get; set; } = []; 
    public IList<Models.Motorista> Motoristas { get; set; } = [];
    public IList<Models.Estudante> Estudantes { get; set; } = [];

    public async Task LoadDataAsync(int id) {
        Viagem = await _context.Viagens
                                .Include(v => v.Motorista)
                                .Include(v => v.Veiculo)
                                .Include(v => v.Estudantes)
                                .Include(v => v.Agendamentos)
                                    .ThenInclude(a => a.Estudante)
                                .FirstOrDefaultAsync(v => v.Id == id);
        Id = id;

        if(_context.Veiculos != null && _context.Motoristas != null && _context.Estudantes != null) {
            Veiculos = await _context.Veiculos.ToListAsync();
            Motoristas = await _context.Motoristas.Include(m => m.Endereco)
                                                  .ToListAsync();
            Estudantes = await _context.Estudantes.Include(e => e.Endereco)
                                                  .ToListAsync();
        }
    }

    public async Task<IActionResult> OnGetAsync(int id) {
        await LoadDataAsync(id);
        if(Viagem == null) return NotFound();

        Input = new InputModel {
            Saida = Viagem.Saida,
            Chegada = Viagem.Chegada,
            MotoristaCnh = Viagem.Motorista.Cnh,
            NumeroRenavam = Viagem.NumeroRenavam
        };

        var agendamentos = Viagem.Agendamentos.ToList();
        var estudantesOrdenados = Estudantes.OrderBy(e => e.Cpf)
                                             .ToList();

        foreach (var estudante in estudantesOrdenados) {
            bool agendado = Viagem.Estudantes.Any(e => e.Cpf == estudante.Cpf);
            DateTime dataAgendada = agendamentos
                                            .Where(a => a.EstudanteId == estudante.Id)
                                            .Select(a => a.Chegada)
                                            .FirstOrDefault();
            InputAgendamentoForms.Add(
                new InputAgendamentoForm {
                    Cpf = estudante.Cpf ?? string.Empty,
                    Checked = agendado,
                    DataHora = dataAgendada
                }
            );
        }

        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync(int id) {
        if (!ModelState.IsValid) {
            await LoadDataAsync(id);
            return Page();
        }

        var viagemEditar = new Viagem {
            Saida = Input.Saida ?? DateTime.Now,
            Chegada = Input.Chegada ?? DateTime.Now.AddHours(2)
        };

        var inputAgendamentos = InputAgendamentoForms.Where(iaf => iaf.Checked)
                                                        .ToList();

        var result = await _viagemService.EditarViagemAsync(id, viagemEditar, Input.MotoristaCnh, Input.NumeroRenavam, inputAgendamentos);

        if(result != null) {
            ModelState.AddModelError(string.Empty, result);
            
            await LoadDataAsync(Id);
            return Page();
        }

        return RedirectToPage("/Viagens/Index", new { area = "Gerente"});
    }

}

public class InputAgendamentoForm {
    [DisplayName("CPF")]
    public required string Cpf { get; set; }
    public bool Checked { get; set; }
    [DisplayName("Chegada")]
    public DateTime DataHora { get; set; }
}