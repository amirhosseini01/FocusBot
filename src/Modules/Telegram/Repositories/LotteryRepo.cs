using FocusBot.Data;
using FocusBot.Data.Repositories;
using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Dto;
using FocusBot.Modules.Telegram.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusBot.Modules.Telegram.Repositories;

public class LotteryRepo(MainContext context) : GenericRepository<Lottery>(context), ILotteryRepo
{
    private readonly DbSet<Lottery> _store = context.Lotteries;

    public async Task<LotterySummaryDto?> GetSummary(string userId)
    {
        return await _store.Select(x => new LotterySummaryDto
        {
            LatestLotteryDate = _store.Where(xx => xx.LotteryDate != null).GroupBy(xx => xx.LotteryDate!.Value).Select(xx => xx.Key).OrderByDescending(xx => xx).FirstOrDefault(),
            QueCount = _store.Where(xx => xx.LotteryDate == null).GroupBy(xx => xx.UserId).Count(),
            WinnerCount = _store.Where(xx => xx.LotteryDate != null && xx.IsWinner && xx.LotteryDate.Value.AddMinutes(TelegramMessages.ValidLotteryDateMinute) > DateTime.Now && xx.ChannelMessageId == null).GroupBy(xx => xx.UserId).Count(),
            CurrentUserLottery = _store.OrderByDescending(xx => xx.Id).FirstOrDefault(xx => xx.UserId == userId),
            MainChannelMessagesCount = _store.Count(xx=> xx.ChannelMessageId > 0 && xx.SendToMainChannelDate != null && xx.DeleteFromMainChannelDate == null),
            InValidMainChannelMessagesCount = _store.Count(xx=> xx.ChannelMessageId > 0 && xx.SendToMainChannelDate != null  && xx.SendToMainChannelDate.Value.AddMinutes(TelegramMessages.ValidMainChannelVoiceMinute) < DateTime.Now),
        }).FirstOrDefaultAsync();
    }

    public async Task<List<Lottery>> GetWinners(CancellationToken ct = default)
    {
        return await _store.Include(x => x.User).Where(x => x.LotteryDate != null && x.SendToMainChannelDate == null && x.SendToArchiveChannelDate == null && x.ChannelMessageId == null && x.ArchiveMessageId == null).OrderBy(x => x.Id).ToListAsync(ct);
    }
    public async Task<List<Lottery>> GetQueueds(CancellationToken ct = default)
    {
        return await _store.Include(x => x.User).Where(x => x.LotteryDate == null).OrderBy(x => x.Id).ToListAsync(ct);
    }
    public async Task<LotteryChannelMessagesDto> GetChannelMessages(CancellationToken ct = default)
    {
        return new LotteryChannelMessagesDto
        {
            Messages = await _store.Include(x => x.User).Where(x=> (x.ArchiveMessageId > 0 && x.SendToArchiveChannelDate != null && x.SendToArchiveChannelDate.Value.AddDays(TelegramMessages.ValidArchiveChannelVoiceDay) < DateTime.Now && x.DeleteFromArchiveChannelDate == null) || (x.ChannelMessageId > 0 && x.SendToMainChannelDate != null && x.SendToMainChannelDate.Value.AddMinutes(TelegramMessages.ValidMainChannelVoiceMinute) < DateTime.Now && x.DeleteFromMainChannelDate == null)).OrderBy(x => x.Id).ToListAsync(ct),
            MainChannelMessageCount = await _store.CountAsync(x=> x.ChannelMessageId > 0 && x.SendToMainChannelDate != null  && x.DeleteFromMainChannelDate == null, ct)
        };
    }
    public async Task<int> GetValidMainChannelMessagesCount(CancellationToken ct = default)
    {
        return await _store.CountAsync(x=> x.ChannelMessageId > 0 && x.SendToMainChannelDate != null && x.SendToMainChannelDate.Value.AddMinutes(TelegramMessages.ValidMainChannelVoiceMinute) > DateTime.Now, cancellationToken: ct);
    }
}