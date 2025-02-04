using MediatR;
using TelegramIfood.Events.Models.Telegram;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.Events.Telegram;
public class TelegramMessageEvent : INotification
{
    public TelegramMessageEvent(CallbackResponse callbackResponse, long id, int? messageResponseId = null)
    {
        CallbackResponse = callbackResponse;
        Id = id;
        MessageResponseId = messageResponseId;
    }

    public TelegramMessageEvent(string mensagem, long id, int? messageResponseId = null)
    {
        CallbackResponse = new CallbackResponse(mensagem, null);
        Id = id;
        MessageResponseId = messageResponseId;
    }


    public CallbackResponse CallbackResponse { get; set; }
    public long Id { get; set; }
    public int? MessageResponseId { get; set; }
}

public class TesteCommand : INotification
{
    public string Mensagem { get; set; }
}
