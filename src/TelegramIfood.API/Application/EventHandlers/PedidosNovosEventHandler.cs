using MediatR;
using System.Runtime.CompilerServices;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.API.Services.Telegram;
using TelegramIfood.API.Utils;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Models.Ifood;
using TelegramIfood.Events.Models.Telegram;

namespace TelegramIfood.API.Application.EventHandlers;

public class PedidosNovosEventHandler : INotificationHandler<PedidosNovosEvent>
{
    private readonly ITelegramSender _telegramSender;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IfoodPedidosService _pedidosService;
    private readonly IIfoodPedidosRepository _pedidosRepository;
    public PedidosNovosEventHandler(ITelegramSender telegramSender,
        IUsuarioRepository usuarioRepository,
        IIfoodPedidosRepository pedidosRepository,
        IfoodPedidosService pedidosService)
    {
        _telegramSender = telegramSender;
        _usuarioRepository = usuarioRepository;
        _pedidosRepository = pedidosRepository;
        _pedidosService = pedidosService;
    }

    public async Task Handle(PedidosNovosEvent notification, CancellationToken cancellationToken)
    {
        var pedidosPorComerciante = notification.Pedidos.GroupBy(x => x.merchantId);

        foreach (var pedidos in pedidosPorComerciante)
        {
            var usuariosComerciante = await _usuarioRepository.GetUsuariosPorMerchantAsync(pedidos.Key);

            if (!usuariosComerciante.Any()) continue;

            foreach (var pedido in pedidos)
            {
                var pedidoDetalhe = await _pedidosService.GetPedidoDetalhesAsync(pedido.orderId);

                if (pedidoDetalhe is null) continue;

                if (await _pedidosRepository.InserirPedidoAsync(pedidoDetalhe))
                {
                    await _telegramSender.SendMessageAsync(GerarMensagemNovoPedido(pedidoDetalhe), usuariosComerciante.Select(x => x.IdTelegram).ToArray());
                }
            }
        }
    }

    private static CallbackResponse GerarMensagemNovoPedido(IfoodPedidoDetalhe pedido)
    {
        return TelegramResponseHelper.GerarResponse(@$"✅ Pedido nº*{pedido.displayId}* recebido ✅

*Valor:* R${pedido.totalPedido:n2}

*Itens do pedido:*

{string.Join('\n', pedido.items.Select(item => $"{item.name} x{item.quantity}\n").ToArray())}")
            .AddLinha()
            .AddBotao("✅ Aceitar pedido", new APedidoEvent(pedido.id))
            .AddBotao(new("❌ Recusar pedido", new CPedidoEvent(pedido.id))).ToResponse();
    }
}