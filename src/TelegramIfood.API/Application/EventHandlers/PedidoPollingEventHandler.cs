using MediatR;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.API.Application.EventHandlers;

public class PedidoPollingEventHandler : INotificationHandler<PedidoPollingEvent>
{
    private readonly IMediator _mediator;
    private readonly IIfoodPedidosRepository _pedidosRepository;
    public PedidoPollingEventHandler(IMediator mediator, IIfoodPedidosRepository pedidosRepository)
    {
        _mediator = mediator;
        _pedidosRepository = pedidosRepository;
    }

    public async Task Handle(PedidoPollingEvent notification, CancellationToken cancellationToken)
    {
        var pollingsPorStatus = notification.Pollings.GroupBy(x => x.code);
        var tasks = new Task[pollingsPorStatus.Count()];

        var i = 0;

        foreach (var status in pollingsPorStatus)
        {
            var handler = status.Key switch
            {
                "PLC" => HandlePedidosCriadosAsync(status),
                "CAN" => HandlePedidosCanceladosAsync(status),
                _ => AtualizarStatusPedidosAsync(status)
            };

            //tasks[i++] = handler;
            await handler;
        }

        //await Task.WhenAll(tasks);
    }

    private async Task HandlePedidosCriadosAsync(IEnumerable<IfoodPedidoPolling> pollings)
    {
        if (!pollings.Any()) return;

        await _mediator.Publish(new PedidosNovosEvent()
        {
            Pedidos = pollings
        });
    }

    private async Task HandlePedidosCanceladosAsync(IEnumerable<IfoodPedidoPolling> pollings)
    {
        if (!pollings.Any()) return;

        await _mediator.Publish(new PedidosCanceladosEvent()
        {
            Pedidos = pollings
        });
    }

    private async Task AtualizarStatusPedidosAsync(IEnumerable<IfoodPedidoPolling> pollings)
    {
        foreach(var polling in pollings)
        {
            var pedido = await _pedidosRepository.GetPedidoByOrderId(polling.orderId);

            if (pedido is null) continue;

            pedido.pedidoStatus = polling.code;

            await _pedidosRepository.AtualizarPedidoAsync(pedido);
        }
    }
}