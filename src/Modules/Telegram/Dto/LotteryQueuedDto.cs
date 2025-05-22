using FocusBot.Modules.Telegram.Models;

namespace FocusBot.Modules.Telegram.Dto;

public class LotteryQueuedDto
{
    public List<Lottery> Winners { get; set; }
    public List<Lottery> Queueds { get; set; }
    public List<Lottery> ChannelMessages { get; set; }
    public int ValidMainChannelMessagesCount { get; set; }
}