using MediatR;
using TelegramIfood.Events.Models.Ifood;

namespace TelegramIfood.Events.Ifood;

public class PedidoPollingEvent : INotification
{
    public IEnumerable<IfoodPedidoPolling> Pollings { get; set; }
}
