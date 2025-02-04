using MediatR;
using Microsoft.VisualBasic;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramIfood.API.Extensions;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.Events.Models.Telegram;
using TelegramIfood.Events.Telegram;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.API.Services.Telegram;
public interface ITelegramSender
{
    Task<Message> SendMessageAsync(CallbackResponse message, long id);
    Task SendMessageAsync(CallbackResponse message, params long[] ids);
    Task SendMessageAsync(CallbackResponse message, IEnumerable<long> ids);
}

public class TelegramSender : ITelegramSender,
    INotificationHandler<TelegramMessageEvent>
{
    private readonly TelegramBotClient botClient;
    public TelegramSender(AppSettings appSettings)
    {
        var telegramToken = appSettings.TelegramSettings.BotToken ?? throw new Exception("Missing TelegramApiKey on appsettings");
        botClient = new(telegramToken);
    }
    public async Task<Message> SendMessageAsync(CallbackResponse message, long id)
    {
        return await botClient.SendTextMessageAsync(
                    chatId: id,
                    text: message.Mensagem,
                    replyMarkup: message.Botoes ?? new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(text: "🗑️ Apagar mensagem", callbackData: "|DELETE|")),
                    parseMode: ParseMode.Markdown
                );
    }

    public async Task SendMessageAsync(CallbackResponse message, params long[] ids)
    {
        var tasks = new Task[ids.Length];

        for (int i = 0; i < ids.Length; i++)
            tasks[i] = SendMessageAsync(message, ids[i]);

        await Task.WhenAll(tasks);
    }

    public async Task SendMessageAsync(CallbackResponse message, IEnumerable<long> ids)
    {
        var tasks = new Task[ids.Count()];

        var i = 0;
        foreach(var id in ids)
            tasks[i++] = SendMessageAsync(message, id);

        await Task.WhenAll(tasks);
    }

    public async Task Handle(TelegramMessageEvent notification, CancellationToken cancellationToken)
    {
        var botaoApagar = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(text: "🗑️ Apagar mensagem", callbackData: "|DELETE|"));

        if (notification.MessageResponseId.HasValue)
        {
            await botClient.EditMessageTextAsync(notification.Id, notification.MessageResponseId.Value, notification.CallbackResponse.Mensagem,
                        replyMarkup: notification.CallbackResponse.Botoes ?? botaoApagar,
                        parseMode: ParseMode.Markdown);
        }
        else
        {
            await botClient.SendTextMessageAsync(
                    chatId: notification.Id,
                    text: notification.CallbackResponse.Mensagem,
                    replyMarkup: notification.CallbackResponse.Botoes ?? botaoApagar,
                    parseMode: ParseMode.Markdown
                );
        }
    }
}
