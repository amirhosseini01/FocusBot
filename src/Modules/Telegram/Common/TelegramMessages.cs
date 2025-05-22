namespace FocusBot.Modules.Telegram.Common;

public static class TelegramMessages
{
    public const string ConfirmVoice = "انتشار در کانال اصلی";
    public const string PreConfirmVoice = "تاییدیه انتشار";
    public const string RegisterLottery = "شرکت در قرعه کشی!";
    public const string DeleteFromMainChannel = "حذف کانال اصلی";
    public const string DeleteFromArchiveChannel = "حذف آرشیو";
    public const string DeleteFromAllChannel = "حذف کانال ها";
    public const string LeftFromLottery = "انصراف قرعه کشی";
    public const string Start = "/start";
    public const string Cancel = "/cancel";
    public const string HideUserName = "/نام-کاربری-مخفی";
    public const string HideVoice = "/صدای-ناشناس";
    public const string FocusChannel = "@focuschanneltest";
    public const string FocusArchiveChannel = "@focusarchivetest";
    public const int ValidLotteryDateMinute = 5;
    public const int ValidVoiceDurationSecond = 600;
    public const int ValidMainChannelVoiceMinute = 3;
    public const int ValidArchiveChannelVoiceDay = 1;
    public const int CouldBeWinnerCount = 1;
    public const int LegalVoiceMessagesInMainChannel = 1;
    /*
        start - شرکت در قرعه کشی 🎉
        cancel - انصراف از قرعه کشی ❌🗑
     */
    public const string LotteryJobCron = "*/1 * * * *";
    public const string FocusChannelName = "اصلی 🎪";
    public const string FocusArchiveChannelName  = "آرشیو 🎥";
    public const string RecurringJobId = "job-id-1";
}