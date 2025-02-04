using MediatR;
using TelegramIfood.Events.Models.Telegram;

namespace TelegramIfood.Events.Telegram;

public class CommandTelegramEvent : INotification
{
    public HookMessage Message { get; set; }
}