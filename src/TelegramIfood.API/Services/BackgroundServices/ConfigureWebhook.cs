﻿using Microsoft.Extensions.Options;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using TelegramIfood.API.Extensions;

namespace TelegramIfood.API.Services.BackgroundServices;

public class ConfigureWebhook : IHostedService
{
    private readonly ILogger<ConfigureWebhook> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotConfiguration _botConfig;

    public ConfigureWebhook(
        ILogger<ConfigureWebhook> logger,
        IServiceProvider serviceProvider,
        AppSettings appSettings)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botConfig = appSettings.TelegramSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        
        var webhookAddress = $"{_botConfig.HostAddress}{_botConfig.Route}";
        _logger.LogInformation("Setting webhook: {WebhookAddress}", webhookAddress);

        await botClient.SetWebhookAsync(
            url: webhookAddress,
            allowedUpdates: Array.Empty<UpdateType>(),
            secretToken: _botConfig.SecretToken,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        _logger.LogInformation("Removing webhook");
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}