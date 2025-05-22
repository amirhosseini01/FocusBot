using FocusBot.Data.Repositories;
using FocusBot.Modules.Telegram.Dto;
using FocusBot.Modules.Telegram.Models;

namespace FocusBot.Modules.Telegram.Repositories;

public interface ILotteryRepo : IGenericRepository<Lottery>
{
    Task<LotterySummaryDto?> GetSummary(string userId);
    Task<int> GetValidMainChannelMessagesCount(CancellationToken ct = default);
    Task<LotteryChannelMessagesDto> GetChannelMessages(CancellationToken ct = default);
    Task<List<Lottery>> GetQueueds(CancellationToken ct = default);
    Task<List<Lottery>> GetWinners(CancellationToken ct = default);
}