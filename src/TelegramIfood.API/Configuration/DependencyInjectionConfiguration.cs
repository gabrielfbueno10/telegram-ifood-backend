using Microsoft.EntityFrameworkCore;
using Polly;
using Telegram.Bot;
using TelegramIfood.API.Data;
using TelegramIfood.API.Data.Repository;
using TelegramIfood.API.Extensions;
using TelegramIfood.API.Services.BackgroundServices;
using TelegramIfood.API.Services.Ifood;
using TelegramIfood.API.Services.Ifood.Handlers;
using TelegramIfood.API.Services.Telegram;

namespace TelegramIfood.API.Configuration;

public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddDIConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddSingleton(x => AppSettings.BuildFromConfiguration(configuration));

        services.AddTransient<IfoodAuthorizationDelegatingHandler>();

        services.AddEventHandlers();

        services.AddScoped<IIfoodPedidosRepository, IfoodPedidosRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IComercianteRepository, EstabelecimentoRepository>();

        services.AddIfoodServices();

        services.AddTransient<ITelegramSender, TelegramSender>();

        services.AddHostedService<ConfigureWebhook>();
        services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    var appSettings = sp.GetService<AppSettings>();
                    var options = new TelegramBotClientOptions(appSettings.TelegramSettings.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

        services.AddHostedService<PedidoPollingBackgroundService>();

        return services;
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        //services.AddScoped<IRequestHandler<InicioCommand, TelegramDefaultResult>, InicioCommandHandler>();

        //services.AddScoped<INotificationHandler<CommandTelegramEvent>, CommandTelegramEventHandler>();

    }

    private static void AddIfoodServices(this IServiceCollection services)
    {
        services.AddHttpClient<IfoodAuthService>()
                .AddHttpMessageHandler<IfoodAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        services.AddHttpClient<IfoodPedidoPollingService>()
                .AddHttpMessageHandler<IfoodAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        services.AddHttpClient<IfoodPedidosService>()
                .AddHttpMessageHandler<IfoodAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

        services.AddHttpClient<IfoodComercioService>()
                .AddHttpMessageHandler<IfoodAuthorizationDelegatingHandler>()
                .AddPolicyHandler(PollyExtensions.EsperarTentar())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
    }
}
