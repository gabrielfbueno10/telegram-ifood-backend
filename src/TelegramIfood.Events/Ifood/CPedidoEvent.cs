using MediatR;

namespace TelegramIfood.Events.Ifood;

public class CPedidoEvent : INotification
{
    public Guid Id { get; set; }

    public CPedidoEvent(Guid orderId)
    {
        Id = orderId;
    }
    public CPedidoEvent() { }
}
