using MediatR;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.API.Utils;
using TelegramIfood.Events.Models.Ifood;
using TelegramIfood.Events.Telegram;
using TelegramIfood.Events.Telegram.Response;

namespace TelegramIfood.API.Application.CommandHandlers;

public class InicioCommandHandler : IRequestHandler<InicioCommand, TelegramDefaultResult>
{
    private readonly IMediator _mediator;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IComercianteRepository _comercianteRepository;
    private readonly IfoodComercioService _comercioService;
    public InicioCommandHandler(IMediator mediator,
        IUsuarioRepository usuarioRepository,
        IComercianteRepository comercianteRepository,
        IfoodComercioService comercioService)
    {
        _mediator = mediator;
        _usuarioRepository = usuarioRepository;
        _comercianteRepository = comercianteRepository;
        _comercioService = comercioService;
    }

    public async Task<TelegramDefaultResult> Handle(InicioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.GetUsuarioPorTelegramIdAsync(request.Id);

        if (usuario is null)
        {

            if (request.Registro.HasValue)
            {
                var estabelecimento = await _comercianteRepository.GetEstabelecimentoAsync(request.Registro.Value);

                if (estabelecimento is null)
                {
                    return new("Chave de registro invalida.", request.Id);
                }
                else
                {
                    var success = await _usuarioRepository.InserirUsuarioAsync(usuario = new()
                    {
                        CreatedAt = DateTime.UtcNow,
                        EstabelecimentoId = estabelecimento.IfoodMerchantId,
                        Ativo = true,
                        IdTelegram = request.Id,
                        Nome = request.Nome,
                    });

                    if (success)
                        await _mediator.Publish(new TelegramMessageEvent("Obrigado por se registrar no Easygram Ifood", request.Id), cancellationToken);
                }
            }
            else
            {
                return new("Você precisa se registrar para acessar nosso bot.", request.Id);
            }
        }

        var comercioStatus = await _comercioService.StatusComercioAsync(usuario.EstabelecimentoId);

        var response = TelegramResponseHelper.GerarResponse(@$"🤖 *Bem-vindo ao Easygram Ifood Bot* 🤖

Status: 
{(comercioStatus.available ? "🟢" : "🔴") + $"* {comercioStatus.message.title}{(string.IsNullOrEmpty(comercioStatus.message.subtitle) ? "" : $" - {comercioStatus.message.subtitle}")}*"}

📋 Selecione abaixo uma opção:");

        response
            .AddLinha().AddBotao("🛒 Pedidos em andamento", new RelatoriosCommand(request.Id,
                        PedidoStatusIfoodConstant.NOVO_PEDIDO_IFOOD,
                        PedidoStatusIfoodConstant.PEDIDO_CONFIRMADO,
                        PedidoStatusIfoodConstant.PEDIDO_PREPARANDO))
            .AddLinha().AddBotao("⌛ Pedidos aguardando", new RelatoriosCommand(request.Id,
                        PedidoStatusIfoodConstant.NOVO_PEDIDO_IFOOD,
                        PedidoStatusIfoodConstant.PEDIDO_CONFIRMADO))
            .AddLinha().AddBotao("👨🏻‍🍳 Pedidos em preparo", new RelatoriosCommand(request.Id,
                        PedidoStatusIfoodConstant.PEDIDO_PREPARANDO))
            .AddLinha().AddBotao("🛵 Pedidos pronto para coleta", new RelatoriosCommand(request.Id,
                        PedidoStatusIfoodConstant.PEDIDO_PRONTO_RETIRAR))
            .AddLinha().AddBotao("📋 Pedidos finalizados hoje", new PhojeCommand(request.Id))
            .AddLinha().AddBotao("⏳ Interrupções", new ListaInterrupcoesCommand(request.Id));

        return new(response.ToResponse(),
            request.Id);
    }
}
