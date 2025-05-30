using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Models;

namespace FocusBot.Modules.Telegram.Dto;

public class LotterySummaryDto
{
    public DateTime? LatestLotteryDate { get; set; }
    public int QueCount { get; set; }
    public int WinnerCount { get; set; }
    public Lottery? CurrentUserLottery { get; set; }
    public bool IsCurrentUserWinner => CurrentUserLottery is not null && CurrentUserLottery.IsWinner && CurrentUserLottery.LotteryDate != null && CurrentUserLottery.Confirmed && CurrentUserLottery.SendToMainChannelDate is null;

    public bool CanRegisterAgain
    {
        get
        {
            if (IsCurrentUserWinner)
            {
                return false;
            }

            if (CurrentUserLottery is null)
            {
                return true;
            }
            
            if (CurrentUserLottery.LotteryDate?.AddMinutes(TelegramMessages.ValidLotteryDateMinute) < DateTime.Now)
            {
                return true;
            }

            if (CurrentUserLottery.SendToMainChannelDate != null && CurrentUserLottery.ChannelMessageId != null)
            {
                return true;
            }

            return false;
        }
    }

    public int MainChannelMessagesCount { get; set; }
    public int InValidMainChannelMessagesCount { get; set; }
}