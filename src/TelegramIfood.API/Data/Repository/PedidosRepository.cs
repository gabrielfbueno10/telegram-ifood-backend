using Microsoft.EntityFrameworkCore;
using TelegramIfood.API.Models.Ifood;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Data.Repository;

public interface IIfoodPedidosRepository
{
    Task<bool> InserirPedidoAsync(IfoodPedidoDetalhe pedido);
    Task<IfoodPedidoDetalhe?> GetPedidoByOrderId(Guid id);
    Task<bool> AtualizarPedidoAsync(IfoodPedidoDetalhe pedido);
    Task<IEnumerable<IfoodPedidoDetalhe>> GetPedidosPorStatusAsync(params string[] status);
    Task<IEnumerable<IfoodPedidoDetalhe>> GetPedidosPorDataAsync(DateTime dataInicio, DateTime? dataFim);
}
public class IfoodPedidosRepository : IIfoodPedidosRepository
{
    private readonly ApplicationDbContext _context;
    public IfoodPedidosRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> InserirPedidoAsync(IfoodPedidoDetalhe pedido)
    {
        pedido.PreencherValoresPedido();
        
        await _context.PedidosIfood.AddAsync(pedido);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IfoodPedidoDetalhe?> GetPedidoByOrderId(Guid id)
    {
        return await _context.PedidosIfood.Include(x => x.items).FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<bool> AtualizarPedidoAsync(IfoodPedidoDetalhe pedido)
    {
        _context.PedidosIfood.Update(pedido);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<IfoodPedidoDetalhe>> GetPedidosPorStatusAsync(params string[] status)
    {
        if (status.Length <= 0) throw new ArgumentOutOfRangeException(nameof(status));

        return await _context.PedidosIfood
            .Include(x => x.items)
            .AsNoTracking()
            .Where(x => status.Any(s => s == x.pedidoStatus))
            .ToListAsync() ?? new List<IfoodPedidoDetalhe>();
    }

    public async Task<IEnumerable<IfoodPedidoDetalhe>> GetPedidosPorDataAsync(DateTime dataInicio, DateTime? dataFim)
    {
        dataFim ??= dataInicio.AddDays(1);

        return await _context.PedidosIfood
            .Include(x => x.items)
            .AsNoTracking()
            .Where(x => x.createdAt >= dataInicio.Date && x.createdAt < dataFim.Value.Date)
            .ToListAsync() ?? new List<IfoodPedidoDetalhe>();
    }

}
