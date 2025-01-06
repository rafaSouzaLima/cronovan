using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Services;

public class EstudanteService {
    private readonly UserManager<Usuario> _userManager;
    private readonly CronovanContext _context;

    public EstudanteService(CronovanContext context, UserManager<Usuario> userManager) {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IdentityResult> CriarEstudanteAsync(Estudante estudante, string senha) {
        if (estudante == null)
            throw new ArgumentNullException(nameof(estudante));
        
        if(estudante.IsUnderage() && estudante.Responsavel == null)
            throw new ArgumentException($"{nameof(estudante.Responsavel)} é nulo quando o estudante é menor de idade!");

        if (!estudante.IsUnderage()) 
            estudante.Responsavel = null;
        
        var resultUsuario = await _userManager.CreateAsync(estudante, senha);
        if(!resultUsuario.Succeeded) return resultUsuario;

        var resultUsuarioRole = await _userManager.AddToRoleAsync(estudante, "Estudante");
        if(!resultUsuarioRole.Succeeded) return resultUsuarioRole;

        if(estudante.IsUnderage()) {
            // Impossível estudante.Responsavel ser nulo
            var resultResponsavelRole = await _userManager.AddToRoleAsync(estudante.Responsavel, "Responsavel");
            if(!resultUsuarioRole.Succeeded) return resultResponsavelRole;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> CriarEstudanteAssociadoAsync(Estudante estudante, string senha, string? cpfResponsavel) {
        if (estudante == null)
            throw new ArgumentNullException(nameof(estudante));


        var responsavel = await _context.Responsaveis.FirstOrDefaultAsync(r => r.Cpf == cpfResponsavel);
        if(responsavel == null)
            throw new ArgumentException($"Responsável com CPF {cpfResponsavel} não foi encontrado");

        estudante.Responsavel = estudante.IsUnderage() ? responsavel : null;
        
        var resultUsuario = await _userManager.CreateAsync(estudante, senha);
        if(!resultUsuario.Succeeded) return resultUsuario;

        var resultUsuarioRole = await _userManager.AddToRoleAsync(estudante, "Estudante");
        if(!resultUsuarioRole.Succeeded) return resultUsuarioRole;

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> AtualizarEstudanteAsync(Estudante estudante) {
        if(estudante == null) {
            throw new ArgumentNullException(nameof(estudante), "O motorista passado não pode ser nulo.");
        }

        var resultAtualizacao = await _userManager.UpdateAsync(estudante);
        if(!resultAtualizacao.Succeeded) return resultAtualizacao;

        return IdentityResult.Success;
    }

    public async Task<List<Estudante>> BuscarEstudantesAsync(string sliceCpf) {
        if(sliceCpf == null) {
            throw new ArgumentNullException(nameof(sliceCpf), "O CPF da busca não pode ser nulo");
        }

        var estudantes = await _userManager.Users.OfType<Estudante>()
                                    .Where(e => e.Cpf != null && e.Cpf.Contains(sliceCpf))
                                    .ToListAsync();
        return estudantes;
    }

    public async Task<IdentityResult> ExcluirEstudanteAsync(string cpf) {
        if(string.IsNullOrWhiteSpace(cpf)) {
            throw new ArgumentNullException(nameof(cpf), "O CPF não pode ser nulo ou vazio.");
        }
    
        var estudante = await _userManager.Users.OfType<Estudante>()
                                .Include(e => e.Responsavel)
                                .FirstOrDefaultAsync(e => e.Cpf == cpf);

        if(estudante == null) {
            return IdentityResult.Failed(
                new IdentityError {
                    Code = "EstudanteNotFound",
                    Description = $"Nenhum estudante com o CPF {cpf}, foi encontrado."
                });
        }

        var responsavel = await _userManager.Users.OfType<Responsavel>()
                                    .Include(r => r.Estudantes)
                                    .FirstOrDefaultAsync(r => r.Cpf == estudante.Responsavel.Cpf);
        
        if(responsavel != null && responsavel.Estudantes.Count == 1) {
            var resultResponsavel = await _userManager.DeleteAsync(responsavel);
        
            if(!resultResponsavel.Succeeded) return resultResponsavel;
        }

        var result = await _userManager.DeleteAsync(estudante);
        if(!result.Succeeded) return result;

        return IdentityResult.Success;
    }
}