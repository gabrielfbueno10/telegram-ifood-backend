using Microsoft.EntityFrameworkCore;
using TelegramIfood.API.Models;
using TelegramIfood.API.Models.Ifood;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
            e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
            property.SetColumnType("varchar(255)");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    public DbSet<IfoodPedidoDetalhe> PedidosIfood { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Estabelecimento> Estabelecimentos { get; set; }

}
