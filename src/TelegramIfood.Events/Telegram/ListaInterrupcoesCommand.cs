using MediatR;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.Events.Telegram;

public class ListaInterrupcoesCommand : IRequest<TelegramDefaultResult>
{
    public long Id { get; set; }

    public ListaInterrupcoesCommand(long id)
    {
        Id = id;
    }
}