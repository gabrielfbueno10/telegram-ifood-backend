using MediatR;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.Events.Telegram;

public class PhojeCommand : IRequest<TelegramDefaultResult>
{
    public PhojeCommand(long id)
    {
        Id = id;
    }
    public long Id { get; set; }
}
