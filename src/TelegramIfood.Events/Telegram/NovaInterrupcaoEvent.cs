using MediatR;

namespace TelegramIfood.Events.Telegram;

public class NovaInterruEvent : INotification
{
    public long Id { get; set; }

    public NovaInterruEvent(long id)
    {
        Id = id;
    }
}
