using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Services;

public class MotoristaService {
    private readonly UserManager<Usuario> _userManager;
    private readonly CronovanContext _context;

    public MotoristaService(CronovanContext context, UserManager<Usuario> userManager) {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IdentityResult> CriarMotoristaAsync(Motorista motorista, string senha) {
        if(motorista == null) {
            throw new ArgumentNullException(nameof(motorista));
        }

        using var transaction = _context.Database.BeginTransaction();

        try {
            var result = await _userManager.CreateAsync(motorista, senha);
            if(!result.Succeeded) {
                transaction.Rollback();
                return result;
            }

            result = await _userManager.AddToRoleAsync(motorista, "Motorista");
            if(!result.Succeeded) {
                transaction.Rollback();
                return result;
            }
            
            transaction.Commit();
        
            return IdentityResult.Success;
        } catch (Exception) {
            transaction.Rollback();
            return IdentityResult.Failed(new IdentityError { Description = "Something went wrong!" });
        }
    }

    public async Task<IdentityResult> AtualizarMotoristaAsync(Motorista motorista) {
        if(motorista == null) {
            throw new ArgumentNullException(nameof(motorista), "O motorista passado não pode ser nulo.");
        }

        using var transaction = _context.Database.BeginTransaction();

        try {
            var result = await _userManager.UpdateAsync(motorista);

            transaction.Commit();

            return result;
        } catch(Exception) {
            transaction.Rollback();
            return IdentityResult.Failed(new IdentityError { Description = "Something went wrong!" });
        }
    }

    public async Task<List<Motorista>> BuscarMotoristasAsync(string sliceCnh) {
        if(sliceCnh == null) {
            throw new ArgumentNullException(nameof(sliceCnh), "O CNH da busca não pode ser nulo");
        }

        var motoristas = await _userManager.Users.OfType<Motorista>()
                                    .Where(m => m.Cnh.Contains(sliceCnh))
                                    .ToListAsync();     
        return motoristas;
    }

    public async Task<IdentityResult> ExcluirMotoristaAsync(string cnh) {
        if(string.IsNullOrWhiteSpace(cnh)) {
            throw new ArgumentNullException(nameof(cnh), "O CNH não pode ser nulo ou vazio.");
        }

        var motorista = await _userManager.Users.OfType<Motorista>()
                                .FirstOrDefaultAsync(m => m.Cnh == cnh);
        
        if(motorista == null) {
            return IdentityResult.Failed(
                new IdentityError {
                    Code = "MotoristaNotFound",
                    Description = $"Nenhum motorista com a CNH {cnh}, foi encontrado."
                });
        }

        var result = await _userManager.DeleteAsync(motorista);
        return result;
    }
}