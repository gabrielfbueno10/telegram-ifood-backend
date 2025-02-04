using Microsoft.EntityFrameworkCore;
using TelegramIfood.API.Models;

namespace TelegramIfood.API.Data.Repository;

public interface IComercianteRepository
{
    Task<Estabelecimento?> GetEstabelecimentoAsync(Guid id);
}
public class EstabelecimentoRepository : IComercianteRepository
{
    private readonly ApplicationDbContext _context;
    public EstabelecimentoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Estabelecimento?> GetEstabelecimentoAsync(Guid id)
    {
        return await _context.Estabelecimentos
            .AsNoTracking()
            .FirstOrDefaultAsync(x=> x.Ativo && x.Id == id);
    }
}