using MediatR;
using System.Text.Json.Serialization;

namespace TelegramIfood.Events.Ifood;

public class PreparoEvent : INotification
{
    public Guid OrderId { get; set; }

    public PreparoEvent(Guid orderId)
    {
        OrderId = orderId;
    }
    public PreparoEvent() { }
}


public class DelInteEvent : NotificationWithFromId, INotification
{
    public Guid Inte { get; set;}
    //public long Id { get; set; }
}

public class NotificationWithFromId
{
    [JsonIgnore]
    public long FromId { get; set; }
}