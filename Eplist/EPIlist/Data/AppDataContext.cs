
using EPIlist.Models;
using Microsoft.EntityFrameworkCore;

namespace Namespace.Data;
public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
    {

    }

    //Classes que vão se tornar tabelas no banco de dados
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Epi> Epis { get; set; }
    public DbSet<Unidade> Unidades { get; set; }
    public DbSet<Equipe> Equipes { get; set; }
    public DbSet<UsuarioEpi> UsuarioEpis { get; set; }
    public DbSet<UnidadeUsuario> UnidadeUsuarios { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurações de entidade, chaves primárias, índices, etc.
        modelBuilder.Entity<UsuarioEpi>()
            .HasKey(ue => new { ue.UsuarioID, ue.EpiID });

        modelBuilder.Entity<UsuarioEpi>()
            .HasOne(ue => ue.Usuario)
            .WithMany(u => u.UsuariosEpis)
            .HasForeignKey(ue => ue.UsuarioID);

        modelBuilder.Entity<UsuarioEpi>()
            .HasOne(ue => ue.Epi)
            .WithMany(e => e.EpisUsuario)
            .HasForeignKey(ue => ue.EpiID);

        // Configurações de entidade, chaves primárias, índices, etc.
        modelBuilder.Entity<UnidadeUsuario>()
            .HasKey(ue => new { ue.UsuarioID, ue.UnidadeID });

        modelBuilder.Entity<UnidadeUsuario>()
            .HasOne(ue => ue.Usuario)
            .WithMany(u => u.UsuariosUnidades)
            .HasForeignKey(ue => ue.UsuarioID);

        modelBuilder.Entity<UnidadeUsuario>()
            .HasOne(ue => ue.Unidade)
            .WithMany(e => e.UnidadesUsuarios)
            .HasForeignKey(ue => ue.UnidadeID);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Equipe) // Um usuário pertence a uma equipe
            .WithMany(e => e.Usuarios) // Uma equipe pode ter muitos usuários
            .HasForeignKey(u => u.EquipeID);

    }
    
}