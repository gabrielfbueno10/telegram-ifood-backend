using MediatR;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.Events.Telegram;

public class InicioCommand : IRequest<TelegramDefaultResult>
{
    public long Id { get; set; }
    public string Nome { get; set; }
    public Guid? Registro { get; set; }
}