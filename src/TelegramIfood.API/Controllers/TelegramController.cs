using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramIfood.API.Attributes;
using TelegramIfood.API.Extensions;
using TelegramIfood.API.Services.Telegram;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Models.Telegram;
using TelegramIfood.Events.Telegram;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.API.Controllers;


[ApiController]
[Route("[controller]")]
public class TelegramController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;
    private readonly IMediator _mediator;
    private readonly ITelegramSender _telegramSender;
    public TelegramController(ILogger<HealthCheckController> logger, IMediator mediator, ITelegramSender telegramSender)
    {
        _logger = logger;
        _mediator = mediator;
        _telegramSender = telegramSender;
    }

    #region ActionHook
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> ReceiveHook(
        [FromServices] ITelegramBotClient botClient,
        [FromBody] object body,
        CancellationToken cts)
    {
        var jsonBody = JsonSerializer.Serialize(body);
        var update = JsonSerializer.Deserialize<TelegramHook>(jsonBody);
        try
        {
            if (update?.message is null && update?.callback_query is null) return Ok();

            if (update.callback_query is not null)
            {
                await botClient.DeleteMessageAsync(update.callback_query.from.id, update.callback_query.message.message_id, cancellationToken: cts);

                if (update.callback_query.data == "|DELETE|")
                {
                    return Ok();
                }
                else if (update.callback_query.data.Contains('|'))
                {
                    var splitData = update.callback_query.data.Split('|');
                    var typeName = splitData[0];

                    if (!EventAssemblyHelper.EventTypes.ContainsKey(typeName)) return Ok();

                    var eventType = EventAssemblyHelper.EventTypes[typeName];
                    var json = splitData[1];

                    if (eventType == EventType.Event)
                    {
                        var type = EventAssemblyHelper.Events[typeName];
                        var @event = JsonSerializer.Deserialize(json, type) ?? throw new Exception($"{json} invalid event");

                        if (@event is NotificationWithFromId e) e.FromId = update.callback_query.from.id;

                        await _mediator.Publish(@event, cts);
                    }
                    else
                    {
                        var type = EventAssemblyHelper.Events[typeName];
                        var command = JsonSerializer.Deserialize(json, type) ?? throw new Exception($"{json} invalid command");

                        if(command is NotificationWithFromId c) c.FromId = update.callback_query.from.id;

                        var response = await _mediator.Send(command, cts);

                        if (response is TelegramDefaultResult commandResult)
                        {
                            var botaoApagar = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(text: "🗑️ Apagar mensagem", callbackData: "|DELETE|"));
                            if (commandResult.MessageResponseId.HasValue)
                            {
                                await botClient.EditMessageTextAsync(commandResult.Id,
                                    commandResult.MessageResponseId.Value,
                                    commandResult.CallbackResponse.Mensagem,
                                    replyMarkup: commandResult.CallbackResponse.Botoes ?? botaoApagar,
                                    parseMode: ParseMode.Markdown,
                                    cancellationToken: cts);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId: commandResult.Id,
                                    text: commandResult.CallbackResponse.Mensagem,
                                    replyMarkup: commandResult.CallbackResponse.Botoes ?? botaoApagar,
                                    parseMode: ParseMode.Markdown,
                                    cancellationToken: cts);
                            }
                        }
                    }

                }
            }
            else
            {
                if (update.message is not { } message)
                    return Ok();

                if (message.text is not { } messageText)
                    return Ok();

                if (messageText.StartsWith('/'))
                {
                    await _mediator.Publish(new CommandTelegramEvent()
                    {
                        Message = message
                    });
                }
                else
                {
                    await _telegramSender.SendMessageAsync(new("Texto não reconhecido", null), message.from.id);
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Erro na service do telegram", e);
        }

        return Ok();
    }
    #endregion
}

