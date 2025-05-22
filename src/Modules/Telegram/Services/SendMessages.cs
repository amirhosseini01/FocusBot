using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FocusBot.Modules.Telegram.Services;

public static class SendMessages
{
    public static async Task SendStartMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                                    جهت شرکت در قرعه کشی ✨ روی دکمه زیر کلیک کنید
                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.RegisterLottery }, cancellationToken: ct);
    }

    public static async Task SendConfirmVoiceMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                                    همه چیز آماده هستش! 😎
                                                    ارسال کنیم تو کانال؟ 🏎
                                                    نکته: میتونی وویس جدید بفرستی تا با قبلی جایگزین بشه
                                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.ConfirmVoice }, cancellationToken: ct);
    }
    
    public static async Task SendPreConfirmVoiceMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                                    همه چیز آماده هستش! 😎
                                                    به محض برنده شدن، ارسال کنیم در کانال؟ 🏎
                                                    نکته: میتونی وویس جدید بفرستی تا با قبلی جایگزین بشه
                                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.PreConfirmVoice }, cancellationToken: ct);
    }

    public static async Task SendSuccessCancelMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    
                                                    انصراف شما با موفقیت انجام شد ✅
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest

                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.RegisterLottery }, cancellationToken: ct);
    }

    public static async Task SendCancelOptionsMessage(ITelegramBotClient bot, Chat chat, ReplyMarkup? replyMarkup, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                        آیا از انصراف اطمینان داری؟ 😭😱
                                    """,
            replyMarkup: replyMarkup,
            cancellationToken: ct);
    }

    public static async Task VoiceSavedSuccessFully(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    وویس شما، با موفقیت ذخیره گردید ✅
                                     بعد از برنده شدن منتشر خواهد شد 🥇
                                    در صورت ارسال مجدد وویس  فایل جدید با قبلی جایگزین خواهد شد 🙄

                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    """,
            cancellationToken: ct);
    }

    public static async Task VoiceReplacedSuccessFully(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    وویس جدید با فایل قبلی جایگزین گردید ✅

                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest

                                    """,
            cancellationToken: ct);
    }

    public static async Task SendAlreadyRegisteredMessage(ITelegramBotClient bot, Chat chat, LotterySummaryDto lotterySummary, CancellationToken ct = default)
    {
        if (lotterySummary.IsCurrentUserWinner)
        {
            await bot.SendMessage(chat, """
                                        
                                                        شما برنده هستید! 🥇
                                                        وویس خود را ارسال نمایید✅
                                                         
                                                         کانال اصلی:
                                        @focuschanneltest

                                        کانال آرشیو:
                                        @focusarchivetest
                                        
                                                    
                                        """, replyMarkup: new InlineKeyboardButton[] { TelegramMessages.ConfirmVoice }, cancellationToken: ct);
        }
        else
        {
            await bot.SendMessage(chat, """
                                        
                                                        شما قبلا در قرعه کشی شرکت کرده اید! ✅
                                                        کمی منتظر بمانید 🥱
                                                         مطلع خواهید شد 😎🥇
                                                         
                                                         کانال اصلی:
                                        @focuschanneltest

                                        کانال آرشیو:
                                        @focusarchivetest
                                        
                                                    
                                        """, cancellationToken: ct);
        }
    }

    public static async Task SendLotteryRegisteredMessage(ITelegramBotClient bot, Update update, Chat chat, LotterySummaryDto lotterySummary, CancellationToken ct = default)
    {
        await bot.AnswerCallbackQuery(update.CallbackQuery!.Id, "شما در قرعه کشی ثبت نام شدید 💪😍🥇", cancellationToken: ct);
        await bot.SendMessage(chat, $"""
                                     
                                                     شما در قرعه کشی ثبت نام شدید 💪😍🥇
                                                     لطفا آنلاین بمانید، قرعه کشی به زودی انجام خواهد شد.
                                                     مطلع خواهید شد 😎🥇
                                                     
                                                     کانال اصلی:
                                     @focuschanneltest

                                     کانال آرشیو:
                                     @focusarchivetest
                                     
                                                 
                                     """, cancellationToken: ct);
    }

    public static async Task SendVoiceOnlyMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    
                                                    شما برنده شده اید 🤑😎🤩
                                                   🤗 فوکس منتظر ارسال وویس(صدا) از شماست.
                                                    اگر نیاز به ناشناس بودن دارید میتوانید از طریق منو انجام دهید 🛠
                                                    
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    
                                                
                                    """, cancellationToken: ct);
    }
    
    public static async Task SendWaitForMessageMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                                    در حال آماده سازی کانال اصالی هستیم. 🤗
                                                    به محض انجام، وویس شما به صورت خودکار در کانال اصلی منتشر خواهد شد
                                                    
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    
                                                
                                    """, cancellationToken: ct);
    }

    public static async Task SendAttemptOnLotteryAgainMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    
                                                    فرصت ارسال وویس شما به پایان رسیده است 💩
                                                    مجددا میتوانید در قرعه کشی شرکت نمایید 😍🙌❤
                                                    
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    
                                                
                                    """, replyMarkup: new InlineKeyboardButton[] { TelegramMessages.RegisterLottery }, cancellationToken: ct);
    }

    public static async Task SendCouldNotForwardMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    
                                                    امکان بازارسال یا forward وجود نداره 🤬😂
                                                    یک وویس از داخل خود ربات ضبط کن ❤🙏
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    
                                                
                                    """, cancellationToken: ct);
    }

    public static async Task SendCantConfirmVoiceMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    امکان ارسال در کانال اصلی وجود ندارد 😊🙏
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest
                                    
                                                
                                    """, cancellationToken: ct);
    }

    public static async Task SendWinnerCongrats(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                    
                                                    تبریک! 🥇🤑😎
                                                     شما برنده قرعه کشی بزرگ فوکس شده اید! 💪😍 
                                                    لطفا وویس خود حداکثر تا 1 دقیقه پس از پیام برنده شدن، برای ما ارسال نمایید. 💎 
                                                    
                                                    کانال اصلی:
                                    @focuschanneltest

                                    کانال آرشیو:
                                    @focusarchivetest

                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.HideUserName, TelegramMessages.HideVoice },
            cancellationToken: ct);
    }

    public static async Task SendSettingUpdateFailedMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                        بروزرسانی تنظیمات با خطا مواجه شد، لطفا دقایقی بعد مجددا تلاش فرمایید.
                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.HideUserName, TelegramMessages.HideVoice },
            cancellationToken: ct);
    }

    public static async Task SendSettingUpdateSuccessMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                        بروزرسانی تنظیمات با موقیت انجام گردید
                                    """,
            replyMarkup: new InlineKeyboardButton[] { TelegramMessages.HideUserName, TelegramMessages.HideVoice },
            cancellationToken: ct);
    }

    public static async Task SendInvalidAudioDurationMessage(ITelegramBotClient bot, Chat chat, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, """
                                        مدت زمان مجاز وویس باید 10 دقیقه میباشد
                                    """,
            cancellationToken: ct);
    }

    public static async Task SendVoiceHasSharedMessage(ITelegramBotClient bot, Chat chat, string channel, CancellationToken ct = default)
    {
        await bot.SendMessage(chat, $"""
                                         تبریک! 🤩🥇💎
                                          پیام شما با موفقیت در کانال {channel} به اشتراک گذاشته شد.
                                         
                                         کانال اصلی:
                                     @focuschanneltest

                                     کانال آرشیو:
                                     @focusarchivetest

                                     """,
            cancellationToken: ct);
    }
}