using FocusBot.Modules.Telegram.Models;

namespace FocusBot.Modules.Telegram.Dto;

public class LotteryChannelMessagesDto
{
    public required List<Lottery> Messages { get; set; }
    public int MainChannelMessageCount { get; set; }
    public int WinnerCount { get; set; }
}