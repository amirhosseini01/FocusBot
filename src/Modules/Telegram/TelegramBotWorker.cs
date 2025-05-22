using FocusBot.Modules.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace FocusBot.Modules.Telegram;

public class TelegramBotWorker(ITelegramBotClient telegramBotClient, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };

        telegramBotClient.StartReceiving(
            updateHandler: UpdateHandler,
            errorHandler: ErrorHandler,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );
        return Task.CompletedTask;
    }

    private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates
        
        using var scope = serviceScopeFactory.CreateScope();
        var handleMessages = scope.ServiceProvider.GetRequiredService<HandleTelegramUpdates>();
        await handleMessages!.Handle(botClient, update, cancellationToken);
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        return Task.CompletedTask;
    }
}