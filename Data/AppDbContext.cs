using Auth.Models;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace Auth.Data;

public class AppDbContext : DbContext
{
    public DbSet<UsuarioModel> Usuarios { get; set; }
    public DbSet<CategoriaModel> Categorias { get; set; }
    public DbSet<ProdutoModel> Produtos { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UsuarioModel>().HasData(
            new UsuarioModel()
            {
                IdUsuario = 1,
                Nome = "Administrador do Sistema",
                Email = "admin@email.com",
                Senha = HashPassword("admin", 10),
                IsAdmin = true
            }
        );
    }
}