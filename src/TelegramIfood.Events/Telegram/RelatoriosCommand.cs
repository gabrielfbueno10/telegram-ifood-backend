using MediatR;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.Events.Telegram;

public class RelatoriosCommand : IRequest<TelegramDefaultResult>
{
    public RelatoriosCommand(long id, params string[] status)
    {
        Id = id;
        Status = status;
    }
    public long Id { get; }
    public string[] Status { get;}
}
