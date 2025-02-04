using MediatR;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.API.Services.Telegram;
using TelegramIfood.API.Utils;
using TelegramIfood.Events.Ifood;

namespace TelegramIfood.API.Application.EventHandlers;

public class PedidoAcoesHandler : INotificationHandler<APedidoEvent>,
                                  INotificationHandler<PreparoEvent>,
                                  INotificationHandler<PColetaEvent>,
                                  INotificationHandler<CPedidoEvent>
{
    private readonly IfoodPedidosService _pedidoService;
    private readonly ITelegramSender _telegramSender;
    private readonly IUsuarioRepository _usuarioRepository;
    public PedidoAcoesHandler(IfoodPedidosService pedidoService, ITelegramSender telegramSender, IUsuarioRepository usuarioRepository)
    {
        _pedidoService = pedidoService;
        _telegramSender = telegramSender;
        _usuarioRepository = usuarioRepository;
    }

    public async Task Handle(CPedidoEvent notification, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.GetPedidoDetalhesAsync(notification.Id);
        var usuariosParaNotificar = (await _usuarioRepository.GetUsuariosPorMerchantAsync(pedido.merchant.id)).Select(x => x.IdTelegram);

        if (!usuariosParaNotificar.Any()) return;

        var response = TelegramResponseHelper.GerarResponse($"❌ Pedido nº*{pedido.displayId}* cancelado com sucesso ❌").ToResponse();

        await _pedidoService.CancelarPedidoAsync(notification.Id);

        await _telegramSender.SendMessageAsync(response, usuariosParaNotificar);
    }

    public async Task Handle(PColetaEvent notification, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.GetPedidoDetalhesAsync(notification.Id);
        var usuariosParaNotificar = (await _usuarioRepository.GetUsuariosPorMerchantAsync(pedido.merchant.id)).Select(x => x.IdTelegram);

        if (!usuariosParaNotificar.Any()) return;

        var response = TelegramResponseHelper.GerarResponse(@$"🛵 Pedido nº*{pedido.displayId}* pronto para a coleta 🛵


⚠️ Quando o entregador chegar nós te avisaremos ⚠️").ToResponse();

        await _pedidoService.PedidoProntoParaColetaAsync(notification.Id);

        await _telegramSender.SendMessageAsync(response, usuariosParaNotificar);
    }

    public async Task Handle(PreparoEvent notification, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.GetPedidoDetalhesAsync(notification.OrderId);
        var usuariosParaNotificar = (await _usuarioRepository.GetUsuariosPorMerchantAsync(pedido.merchant.id)).Select(x => x.IdTelegram);

        if (!usuariosParaNotificar.Any()) return;

        var response = TelegramResponseHelper.GerarResponse($"👨🏻‍🍳 Pedido nº*{pedido.displayId}* está sendo praparado 👨🏻‍🍳")
            .AddLinha().AddBotao("📦 Pedido pronto para coleta", new PColetaEvent(notification.OrderId)).ToResponse();

        await _pedidoService.ComecarPreparacaoAsync(notification.OrderId);

        await _telegramSender.SendMessageAsync(response, usuariosParaNotificar);
    }

    public async Task Handle(APedidoEvent notification, CancellationToken cancellationToken)
    {
        var pedido = await _pedidoService.GetPedidoDetalhesAsync(notification.Id);
        var usuariosParaNotificar = (await _usuarioRepository.GetUsuariosPorMerchantAsync(pedido.merchant.id)).Select(x => x.IdTelegram);

        if (!usuariosParaNotificar.Any()) return;

        var response = TelegramResponseHelper.GerarResponse($"🎉 Pedido nº*{pedido.displayId}* recebido com sucesso 🎉")
            .AddLinha().AddBotao("👨🏻‍🍳 Iniciar preparo", new PreparoEvent(notification.Id))
            .AddLinha().AddBotao("📦 Pedido pronto para coleta", new PColetaEvent(notification.Id)).ToResponse();

        await _pedidoService.ConfirmarPedidoAsync(notification.Id);

        await _telegramSender.SendMessageAsync(response, usuariosParaNotificar);
    }
}
