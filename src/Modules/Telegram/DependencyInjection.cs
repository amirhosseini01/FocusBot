using FocusBot.Modules.Telegram.Repositories;
using FocusBot.Modules.Telegram.Services;
using Telegram.Bot;

namespace FocusBot.Modules.Telegram;

public static class DependencyInjection
{
    public static void AddTelegram(this WebApplicationBuilder builder)
    {
        
        var token = builder.Configuration["Telegram:Token"];
        
        builder.Services.AddHostedService<TelegramBotWorker>();
        builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token!));
        builder.Services.AddScoped<HandleTelegramUpdates>();
        builder.Services.AddScoped<ILotteryRepo, LotteryRepo>();

    }
}