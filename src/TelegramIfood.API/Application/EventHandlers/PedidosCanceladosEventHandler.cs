using MediatR;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Services.Telegram;
using TelegramIfood.API.Utils;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Models.Ifood;
using TelegramIfood.Events.Models.Telegram;

namespace TelegramIfood.API.Application.EventHandlers;

public class PedidosCanceladosEventHandler : INotificationHandler<PedidosCanceladosEvent>
{
    private readonly IIfoodPedidosRepository _pedidosRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITelegramSender _telegramSender;
    public PedidosCanceladosEventHandler(IUsuarioRepository usuarioRepository, IIfoodPedidosRepository pedidosRepository, ITelegramSender telegramSender)
    {
        _usuarioRepository = usuarioRepository;
        _pedidosRepository = pedidosRepository;
        _telegramSender = telegramSender;
    }

    public async Task Handle(PedidosCanceladosEvent notification, CancellationToken cancellationToken)
    {
        var pedidosPorComerciante = notification.Pedidos.GroupBy(x => x.merchantId);

        foreach (var pedidos in pedidosPorComerciante)
        {
            var usuariosComerciante = await _usuarioRepository.GetUsuariosPorMerchantAsync(pedidos.Key);

            foreach (var pedido in pedidos)
            {
                var pedidoDetalhe = await _pedidosRepository.GetPedidoByOrderId(pedido.orderId);

                if (pedidoDetalhe is null) continue;

                pedidoDetalhe.pedidoStatus = PedidoStatusIfoodConstant.PEDIDO_CANCELADO;

                if (await _pedidosRepository.AtualizarPedidoAsync(pedidoDetalhe))
                {
                    await _telegramSender.SendMessageAsync(GerarMensagemPedidoCancelado(pedidoDetalhe), usuariosComerciante.Select(x => x.IdTelegram).ToArray());
                }
            }
        }
    }

    private static CallbackResponse GerarMensagemPedidoCancelado(IfoodPedidoDetalhe pedido)
    {
        return TelegramResponseHelper.GerarResponse(@$"❌ Pedido nº*{pedido.displayId}* CANCELADO ❌

*Valor:* R${pedido.totalPedido:n2}

*Itens do pedido:*

{string.Join('\n', pedido.items.Select(item => $"{item.name} x{item.quantity}\n").ToArray())}").ToResponse();
    }
}