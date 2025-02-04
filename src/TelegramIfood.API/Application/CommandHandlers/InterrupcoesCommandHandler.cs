using MediatR;
using System.Text;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.API.Services.Telegram;
using TelegramIfood.API.Utils;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Telegram;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.API.Application.CommandHandlers;

public class InterrupcoesCommandHandler : IRequestHandler<ListaInterrupcoesCommand, TelegramDefaultResult>,
    INotificationHandler<NovaInterruEvent>,
    INotificationHandler<DelInteEvent>
{
    private readonly IfoodComercioService _comercioService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITelegramSender _telegramSender;
    public InterrupcoesCommandHandler(IfoodComercioService comercioService, IUsuarioRepository usuarioRepository, ITelegramSender telegramSender)
    {
        _comercioService = comercioService;
        _usuarioRepository = usuarioRepository;
        _telegramSender = telegramSender;
    }

    public async Task<TelegramDefaultResult> Handle(ListaInterrupcoesCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetUsuarioPorTelegramIdAsync(request.Id);
        var interrupcoes = await _comercioService.ListarInterrupcoesAsync(usuario.EstabelecimentoId);
        var response = TelegramResponseHelper.GerarResponse("");


        if (interrupcoes.Any())
        {
            var msg = new StringBuilder();

            msg.Append("Segue abaixo a lista de interrupções programadas para o seu comercio:\n");


            foreach (var interrupcao in interrupcoes)
            {
                msg.Append($"\n{interrupcao.description}\n{interrupcao.start:dd:MM HH:mm} - {interrupcao.end:dd:MM HH:mm}");

                response.AddLinha().AddBotao($"{interrupcao.description}", "xxx")
                    .AddBotao("❌", new DelInteEvent() { Inte = interrupcao.id});

            }

            response.AtualizarMensagem(msg.ToString());

            return new(response.ToResponse(), request.Id);
        }
        else
        {
            response.AtualizarMensagem("Nenhuma interrupção encontrada para o seu comercio");
        }

        response.AddLinha().AddBotao("➕ Criar interrupção", new NovaInterruEvent(request.Id));

        return new(response.ToResponse(), request.Id);
    }

    public async Task Handle(NovaInterruEvent notification, CancellationToken cancellationToken)
    {
        await _telegramSender.SendMessageAsync(new Events.Models.Telegram.CallbackResponse("Para criar uma interrupção no seu estabelecimento é muito simples, " +
            "basta utilizar o comando */interrupcao* adicionar um \"|\" digitar o motivo da interrupção \"|\" a quantia de horas que deseja interromper o funcionamento" +
            "\n\n*Exemplo:* `/interrupcao|Feriado do ano novo|24`\n(Você pode clicar no exemplo acima para copiar)"), notification.Id);
    }

    public async Task Handle(DelInteEvent notification, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetUsuarioPorTelegramIdAsync(notification.FromId);

        await _comercioService.DeletarInterruptionAsync(usuario.EstabelecimentoId, notification.Inte);

        await _telegramSender.SendMessageAsync(new("Interrupção deletada com sucesso."), notification.FromId);
    }
}