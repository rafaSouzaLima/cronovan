using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using webapp.Models;
using Microsoft.AspNetCore.Identity;

namespace webapp.Data;

public class CronovanContext : IdentityDbContext<Usuario> {
    public CronovanContext(DbContextOptions<CronovanContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Name = "Gerente", NormalizedName = "GERENTE" },
            new IdentityRole { Name = "Estudante", NormalizedName = "ESTUDANTE" },
            new IdentityRole { Name = "Responsavel", NormalizedName = "RESPONSAVEL" },
            new IdentityRole { Name = "Motorista", NormalizedName = "MOTORISTA" }
        );
    }
}