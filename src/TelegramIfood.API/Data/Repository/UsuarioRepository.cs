using Microsoft.EntityFrameworkCore;
using TelegramIfood.API.Models;

namespace TelegramIfood.API.Data.Repository;

public interface IUsuarioRepository
{
    Task<IEnumerable<Usuario>> GetUsuariosPorMerchantAsync(Guid merchantId);
    Task<bool> InserirUsuarioAsync(Usuario usuario);
    Task<Usuario?> GetUsuarioPorTelegramIdAsync(long idTelegram);
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;
    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Usuario>> GetUsuariosPorMerchantAsync(Guid merchantId)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .Where(x=> x.EstabelecimentoId == merchantId).ToListAsync() ?? new List<Usuario>();
    }

    public async Task<bool> InserirUsuarioAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Usuario?> GetUsuarioPorTelegramIdAsync(long idTelegram)
    {
        return await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.IdTelegram == idTelegram);
    }
}
