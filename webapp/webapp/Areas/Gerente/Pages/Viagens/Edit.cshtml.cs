using System.ComponentModel;
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
class EditModel : PageModel {
    private readonly ILogger<CreateModel> _logger;
    private readonly ViagemService _viagemService;
    private readonly CronovanContext _context;

    public EditModel(ILogger<CreateModel> logger, ViagemService viagemService, CronovanContext context) {
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

    public async Task<IActionResult> OnGetAsync(int? id) {
        if(id == null) return NotFound();
        await LoadDataAsync((int)id);
        if(Viagem == null) return NotFound();

        Input = new InputModel {
            Saida = Viagem.Saida,
            Chegada = Viagem.Chegada,
            MotoristaCnh = Viagem.Motorista.Cnh,
            NumeroRenavam = Viagem.NumeroRenavam
            // Cpfs = agendamentosOrdenados.Select(a => a.Estudante.Cpf ?? "").ToList(),
            // AgendamentosHorario = agendamentosOrdenados.Select(a => a.Chegada).ToList()
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
                    Nome = estudante.Nome ?? string.Empty,
                    Email = estudante.Email ?? string.Empty,
                    DataNascimento = estudante.DataNascimento,
                    Telefone = estudante.Telefone ?? string.Empty,
                    Endereco = estudante.Endereco?.ToString() ?? string.Empty,
                    Checked = agendado,
                    DataHora = dataAgendada
                }
            );
        }

        return Page(); 
    }

    public async Task<IActionResult> OnPostAsync(int? id) {
        var viagemEditar = new Viagem {
            Saida = Input.Saida,
            Chegada = Input.Chegada
        };

        var inputAgendamentos = InputAgendamentoForms.Where(iaf => iaf.Checked)
                                                        .ToList();

        var result = await _viagemService.EditarViagemAsync((int)id, viagemEditar, Input.MotoristaCnh, Input.NumeroRenavam, inputAgendamentos);

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
    [DisplayName("Nome")]
    public required string Nome { get; set; }
    [DisplayName("E-mail")]
    public required string Email { get; set; }
    [DisplayName("Data de Nascimento")]
    [DataType(DataType.Date)]
    public required DateTime DataNascimento { get; set; }
    [DisplayName("Telefone")]
    public required string Telefone { get; set; }
    [DisplayName("Endereço")]
    public required string Endereco { get; set; }
    public bool Checked { get; set; }
    [DisplayName("Chegada")]
    public DateTime DataHora { get; set; }
}