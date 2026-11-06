using Microsoft.EntityFrameworkCore;
using nitynews.Model;

namespace nitynews.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            
            // Índice único para garantir que não haja e-mails duplicados a nível de banco
            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.Nome)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(254);

            entity.Property(u => u.Ativo)
                  .IsRequired();

            entity.Property(u => u.EmailVerificado)
                  .IsRequired();

            entity.Property(u => u.DataCadastro)
                  .IsRequired();
        });
    }
}
