using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FocusBot.Pages;

public class IndexModel : PageModel
{
    public IndexModel()
    {
        RecurringJob.AddOrUpdate<LotteryBackground>(
            TelegramMessages.RecurringJobId,
            service => service.DoLottery(),
            TelegramMessages.LotteryJobCron
        );

    }
    public async Task OnGet(CancellationToken ct = default)
    {

    }
}