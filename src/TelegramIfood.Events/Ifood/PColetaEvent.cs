using MediatR;

namespace TelegramIfood.Events.Ifood;

public class PColetaEvent : INotification
{
    public PColetaEvent() { }
    public PColetaEvent(Guid orderId)
    {
        Id = orderId;
    }
    public Guid Id { get; set; }
}