using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramIfood.Events.Models.Telegram;

namespace TelegramIfood.Events.Telegram.Response;

public class TelegramDefaultResult
{
    public TelegramDefaultResult(CallbackResponse callbackResponse, long id, int? messageResponseId = null)
    {
        CallbackResponse = callbackResponse;
        Id = id;
        MessageResponseId = messageResponseId;
    }
    public TelegramDefaultResult(string mensagem, long id, int? messageResponseId = null)
    {
        CallbackResponse = new(mensagem);
        Id = id;
        MessageResponseId = messageResponseId;
    }
    public TelegramDefaultResult() { }

    public CallbackResponse CallbackResponse { get; set; }
    public long Id { get; set; }
    public int? MessageResponseId { get; set; }
}
