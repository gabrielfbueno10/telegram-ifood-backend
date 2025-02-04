using MediatR;

namespace TelegramIfood.Events.Ifood;

public class APedidoEvent : INotification
{
    public Guid Id { get; set; }
    public APedidoEvent(Guid orderId)
    {
        Id = orderId;
    }
    public APedidoEvent() { }
}
