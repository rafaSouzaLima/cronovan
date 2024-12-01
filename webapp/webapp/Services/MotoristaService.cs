using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webapp.Data;
using webapp.Models;

namespace webapp.Services;

public class MotoristaService {
    private readonly UserManager<Usuario> _userManager;

    public MotoristaService(UserManager<Usuario> userManager) {
        _userManager = userManager;
    }

    public async Task<IdentityResult> CriarMotoristaAsync(Motorista motorista, string senha) {
        if(motorista == null) {
            throw new ArgumentNullException(nameof(motorista));
        }

        var result = await _userManager.CreateAsync(motorista, senha);
        if(!result.Succeeded) return result;

        result = await _userManager.AddToRoleAsync(motorista, "Motorista");
        if(!result.Succeeded) return result;

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> AtualizarMotoristaAsync(Motorista motorista) {
        if(motorista == null) {
            throw new ArgumentNullException(nameof(motorista), "O motorista passado não pode ser nulo.");
        }

        var result = await _userManager.UpdateAsync(motorista);
        return result;
    }

    public async Task<List<Motorista>> BuscarMotoristasAsync(string sliceCnh) {
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