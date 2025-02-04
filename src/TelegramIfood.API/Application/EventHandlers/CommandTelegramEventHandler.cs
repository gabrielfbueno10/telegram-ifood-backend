using MediatR;
using Telegram.Bot.Types;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.API.Services.Telegram;
using TelegramIfood.Events.Models.Telegram;
using TelegramIfood.Events.Telegram;

namespace TelegramIfood.API.Application.EventHandlers;

public class CommandTelegramEventHandler : INotificationHandler<CommandTelegramEvent>
{
    private readonly IMediator _mediator;
    private readonly ITelegramSender _telegramSender;
    private readonly IfoodComercioService _comercioService;
    private readonly IUsuarioRepository _usuarioRepository;
    public CommandTelegramEventHandler(IMediator mediator, ITelegramSender telegramSender, IfoodComercioService comercioService, IUsuarioRepository usuarioRepository)
    {
        _mediator = mediator;
        _telegramSender = telegramSender;
        _comercioService = comercioService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task Handle(CommandTelegramEvent notification, CancellationToken cancellationToken)
    {
        var message = notification.Message;
        var command = message.text.Split(new string[] { " ", "|" }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
        var handler = command switch
        {
            "/start" => StartHandler(message),
            "/interrupcao" => NovaInterrupcaoHandler(message),
            _ => ComandoInvalidoHandler(message),
        };

        await handler;
    }

    private async Task StartHandler(HookMessage message)
    {
        var split = message.text.Split(' ');

        var start = await _mediator.Send(new InicioCommand()
        {
            Id = message.from.id,
            Nome = message.from.first_name,
            Registro = split?.Length > 1 && Guid.TryParse(split[1], out var registro) ? registro : null
        });

        await _telegramSender.SendMessageAsync(start.CallbackResponse, start.Id);
    }

    private async Task NovaInterrupcaoHandler(HookMessage message)
    {
        var usuario = await _usuarioRepository.GetUsuarioPorTelegramIdAsync(message.from.id);
        var split = message.text.Split('|');

        if (split.Length == 3 && int.TryParse(split[2], out var qtdHoras))
        {
            var end = DateTime.UtcNow.AddHours(qtdHoras);
            await _comercioService.CriarInterrupcaoAsync(usuario.EstabelecimentoId, DateTime.UtcNow, end, split[1]);

            await _telegramSender.SendMessageAsync(new($"Interrupção programada com sucesso até {end.ToLocalTime():dd/MM/yy HH:mm}"), message.from.id);
        }
        else
        {
            await _telegramSender.SendMessageAsync(new CallbackResponse("Comando invalido\n\nPara criar uma interrupção no seu estabelecimento é muito simples, " +
            "basta utilizar o comando */interrupcao* adicionar um \"|\" digitar o motivo da interrupção \"|\" a quantia de horas que deseja interromper o funcionamento" +
            "\n\n*Exemplo:* `/interrupcao|Feriado do ano novo|24`\n(Você pode clicar no exemplo acima para copiar)"), message.from.id);
        }
    }

    private async Task ComandoInvalidoHandler(HookMessage message)
    {
        await _telegramSender.SendMessageAsync(new("Comando invalido", null), message.from.id);
    }
}
