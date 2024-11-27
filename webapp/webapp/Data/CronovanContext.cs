using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using webapp.Models;
using Microsoft.AspNetCore.Identity;

namespace webapp.Data;

public class CronovanContext : IdentityDbContext<Usuario> {
    public CronovanContext(DbContextOptions<CronovanContext> options) : base(options) {}

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Gerente> Gerentes { get; set; }
    public DbSet<Responsavel> Responsaveis { get; set; }
    public DbSet<Estudante> Estudantes { get; set; }
    public DbSet<Motorista> Motoristas { get; set; }
    public DbSet<Endereco> Enderecos { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        /*
            É preciso definir o lado dominante da relação entre
            o usuário e o endereço, como o endereço tendo a chave
            estrangeira para o usuário. A relação é One-to-one 
            required.
        */
        builder.Entity<Endereco>()
               .HasOne(e => e.Usuario)
               .WithOne(u => u.Endereco)
               .HasForeignKey<Endereco>(e => e.UsuarioId)
               .IsRequired();

        var roles = new IdentityRole[]
        {
            new IdentityRole { Id = "admin-role-id", Name = "Administrador", NormalizedName = "ADMINISTRADOR" },
            new IdentityRole { Id = "manager-role-id", Name = "Gerente", NormalizedName = "GERENTE" },
            new IdentityRole { Id = "student-role-id", Name = "Estudante", NormalizedName = "ESTUDANTE" },
            new IdentityRole { Id = "responsible-role-id", Name = "Responsavel", NormalizedName = "RESPONSAVEL" },
            new IdentityRole { Id = "driver-role-id", Name = "Motorista", NormalizedName = "MOTORISTA" }
        };

        builder.Entity<IdentityRole>().HasData(roles);
        
        // TODO Também é bom futuramente colocar esses dados nas variáveis de ambiente
        // do ambiente de produção
        // O nome disso é Data Seeding e é bom ver a recomendação da Microsoft para dados dinâmicos
        // como nesse caso, que usa hash. Porém as melhores recomendações ou pedem para isso ser
        // feito diretamente no código da migração, ou colocar isso nos métodos novos de seeding
        // do EF9 Core.
        var hasher = new PasswordHasher<Usuario>();
        var defaultAdminUser = new Usuario {
            Id = "05f11b41-2f6a-4033-a264-90355f4145f2",
            UserName = "admin@admin.com", 
            Email = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            NormalizedEmail = "ADMIN@ADMIN.COM"
        };
        defaultAdminUser.PasswordHash = hasher.HashPassword(defaultAdminUser, "admin");
        builder.Entity<Usuario>(b => {
            b.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }); 
        builder.Entity<Usuario>().HasData(defaultAdminUser);

        /*
            Adiciona o usuário criado aos administradores pela tabela intermediária entre
            Usuario e IdentityUserRole.
        */
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> {
                UserId = defaultAdminUser.Id,
                RoleId = roles.First(r => r.Name == "Administrador").Id
            }
        );
    }
}