using MediatR;
using System.Text;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Utils;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Models.Ifood;
using TelegramIfood.Events.Telegram;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.API.Application.CommandHandlers;

public class RelatorioStatusPedidosCommandHandler : IRequestHandler<RelatoriosCommand, TelegramDefaultResult>,
                                                    IRequestHandler<PhojeCommand, TelegramDefaultResult>
{
    private readonly IIfoodPedidosRepository _pedidosRepository;
    public RelatorioStatusPedidosCommandHandler(IIfoodPedidosRepository pedidosRepository)
    {
        _pedidosRepository = pedidosRepository;
    }

    public async Task<TelegramDefaultResult> Handle(RelatoriosCommand request, CancellationToken cancellationToken)
    {
        var pedidos = await _pedidosRepository.GetPedidosPorStatusAsync(request.Status);

        if (!pedidos.Any())
        {
            return new("⚠️ Nenhum pedido no relatorio solicitado ⚠️", request.Id);
        }
        var msg = new StringBuilder();

        msg.Append($"Lista dos pedidos solicitados:\n\n");

        msg.Append("❌ - *Recusar pedido*\n✅ - *Confirmar pedido*\n📦 - *Pedido pronto para coleta*\n\n");

        msg.AppendJoin("\n━━━━━━━━━━━━━━━━━━━━\n", pedidos.Select(pedido => $"Pedido nº*{pedido.displayId}*\nValor: R$*{pedido.totalPedido}*\n" +
        $"{string.Join('\n', pedido.items.Select(item => $"{item.name} x{item.quantity}").ToArray())}").ToArray());

        var response = TelegramResponseHelper.GerarResponse(msg.ToString());

        foreach (var pedido in pedidos)
        {
            response = response.AddLinha()
                .AddBotao($"{pedido.displayId}", "xxx")
                .AddBotao("❌", new CPedidoEvent(pedido.id));

            if(pedido.pedidoStatus == PedidoStatusIfoodConstant.NOVO_PEDIDO_IFOOD)
                response = response.AddBotao("✅", new APedidoEvent(pedido.id));
            
            if (pedido.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_PREPARANDO || pedido.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CONFIRMADO)
                response = response.AddBotao("📦", new PColetaEvent(pedido.id));
            
            if (pedido.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CONFIRMADO)
                response = response.AddBotao("👨🏻‍🍳", new PreparoEvent(pedido.id));
            
        }

        return new(response.ToResponse(), request.Id);
    }

    public async Task<TelegramDefaultResult> Handle(PhojeCommand request, CancellationToken cancellationToken)
    {
        var pedidos = (await _pedidosRepository.GetPedidosPorDataAsync(DateTime.UtcNow.Date, null))
            .Where(x=> x.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CONCLUIDO ||
                       x.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CANCELADO);

        if (!pedidos.Any())
        {
            return new("🙁 Nenhum pedido realizado hoje até agora", request.Id);
        }
        var totalPedidos = pedidos.Where(x => x.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CONCLUIDO).Sum(x => x.totalPedido);

        var msg = new StringBuilder();

        msg.Append("📋 Relatório de vendas *HOJE* 📋\n\n");

        msg.Append($"Total em vendas: *R${totalPedidos:n2}*\nTotal de pedidos: {pedidos.Count()}\n");

        msg.Append($"Pedidos cancelados: *{pedidos.Where(x => x.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CANCELADO).Count()}*\n");
        msg.Append($"Pedidos concluidos: *{pedidos.Where(x => x.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CONCLUIDO).Count()}*\n\n");

        msg.Append("*Status:*\n*✅ - Concluido*\n*❌ - Cancelado*\n\n");

        msg.Append($"*Pedidos:*\n");

        msg.AppendJoin('\n', pedidos.Select(pedido => $"{(pedido.pedidoStatus == PedidoStatusIfoodConstant.PEDIDO_CONCLUIDO ? "✅" : "❌")} nº*{pedido.displayId}* *R${pedido.totalPedido}* *{pedido.items.Sum(x => x.quantity)}* itens").ToArray());

        return new(msg.ToString(), request.Id);
    }
}
