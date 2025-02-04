using MediatR;
using TelegramIfood.API.Extensions;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.Events.Ifood;
using TelegramIfood.Events.Telegram;

namespace TelegramIfood.API.Services.BackgroundServices;

public class PedidoPollingBackgroundService : BackgroundService
{
    private static bool _lock = false;
    private readonly ILogger<PedidoPollingBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IfoodPedidoPollingService _pollingService;
    private readonly IfoodAuthService _ifoodAuthService;
    private Timer? _timer;
    public PedidoPollingBackgroundService(ILogger<PedidoPollingBackgroundService> logger,
        IServiceProvider serviceProvider,
        IfoodPedidoPollingService pollingService,
        IfoodAuthService ifoodAuthService)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _pollingService = pollingService;
        _ifoodAuthService = ifoodAuthService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Serviço de pedidos iniciado.");

        _timer = new Timer(ProcessarPollingAsync, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(4));

        return Task.CompletedTask;
    }

    private async void ProcessarPollingAsync(object? state)
    {
        if (_lock) return;
        _lock = true;

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetService<IMediator>() ?? throw new Exception("Error on activating Mediatr on pedidoPollingBackgroundService");

        try
        {
            if (!IfoodTokenValidatorExtension.EstaValido())
            {
                var auth = await _ifoodAuthService.CreateSessionAsync();
            };

            var pedidos = await _pollingService.GetPedidoPollingsAsync();
            if (!pedidos.Any()) return;

            await _pollingService.EnviarRecebimentoPedidoAsync(pedidos);

            await mediator.Publish(new PedidoPollingEvent()
            {
                Pollings = pedidos
            });
        }
        catch (Exception error)
        {
            _logger.LogError("Erro na pedido pollingBackgrounService {Error}", error);
        }
        finally
        {
            _lock = false;
        }
    }
}
