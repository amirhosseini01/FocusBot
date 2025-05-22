using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Dto;
using FocusBot.Modules.Telegram.Models;
using FocusBot.Modules.User.Models;
using Microsoft.AspNetCore.Identity;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FocusBot.Modules.Telegram.Conditions;

public static class LotteryConditions
{
    public static bool IsHandleVoiceAllowed(this LotterySummaryDto lotterySummary, Message msg)
    {
        return lotterySummary.CurrentUserLottery is not null && msg.Type == MessageType.Voice;
    }

    public static bool IsOnlyVoiceAllowed(this LotterySummaryDto lotterySummary, Message msg)
    {
        if (!lotterySummary.IsCurrentUserWinner)
        {
            return false;
        }

        if (msg is { Type: MessageType.Voice, Voice: not null })
        {
            return false;
        }

        return true;
    }

    public static bool IsLotteryDateExpired(this Lottery currentLottery)
    {
        return currentLottery.LotteryDate?.AddMinutes(TelegramMessages.ValidLotteryDateMinute) < DateTime.Now;
    }

    public static bool IsInValidDuration(this Voice voice)
    {
        return voice.Duration > TelegramMessages.ValidVoiceDurationSecond;
    }

    public static bool IsHideVoice(this IdentityUserCustom user)
    {
        return user.HideVoice;
    }

    public static bool IsNeedSendToChannels(this LotterySummaryDto lotterySummary)
    {
        if (!lotterySummary.IsCurrentUserWinner)
        {
            return false;
        }

        if (lotterySummary.CurrentUserLottery is null)
        {
            return false;
        }

        if (lotterySummary.CurrentUserLottery.LotteryDate is null)
        {
            return false;
        }

        if (lotterySummary.CurrentUserLottery.LotteryDate.Value.AddMinutes(TelegramMessages.ValidLotteryDateMinute) < DateTime.Now)
        {
            return false;
        }

        if (!lotterySummary.CurrentUserLottery.Confirmed)
        {
            return false;
        }

        if (lotterySummary.InValidMainChannelMessagesCount > 0 && lotterySummary.MainChannelMessagesCount > 0)
        {
            return true;
        }

        if (lotterySummary.MainChannelMessagesCount > 0)
        {
            return false;
        }
        
        return true;
    }

    public static bool IsForwarded(this Message msg)
    {
        if (msg.ForwardDate is not null)
        {
            return true;
        }
        
        if (msg.ForwardFrom is not null)
        {
            return true;
        }
        
        if (msg.ForwardOrigin is not null)
        {
            return true;
        }
        
        if (msg.ForwardSignature is not null)
        {
            return true;
        }
        
        if (msg.ForwardFromChat is not null)
        {
            return true;
        }
        
        if (msg.ForwardSenderName is not null)
        {
            return true;
        }
        
        if (msg.ForwardFromMessageId is not null)
        {
            return true;
        }
        
        if (msg.IsAutomaticForward)
        {
            return true;
        }

        return false;
    }

    public static bool IsLotteryDateExpired(this LotterySummaryDto lotterySummary)
    {
        return lotterySummary.LatestLotteryDate is not null && lotterySummary.LatestLotteryDate.Value.AddMinutes(TelegramMessages.ValidLotteryDateMinute) < DateTime.Now;
    }

    public static bool IsVoiceShared(this Lottery currentLottery, out int? voiceMessageId)
    {
        if (currentLottery.VoiceMessageId is null)
        {
            voiceMessageId = 0;
            return false;
        }

        voiceMessageId = currentLottery.VoiceMessageId.Value;
        return true;
    }

    public static bool IsCanRegisterAgain(this LotterySummaryDto lotterySummary)
    {
        return lotterySummary.CanRegisterAgain;
    }

    public static bool IsRegisteredUserCouldBeWinner(this LotterySummaryDto lotterySummary)
    {
        return lotterySummary.QueCount < TelegramMessages.CouldBeWinnerCount;
    }

    public static bool IsHideVoice(this string data)
    {
        return data == TelegramMessages.HideVoice;
    }

    public static bool IsHideUserName(this string data)
    {
        return data == TelegramMessages.HideUserName;
    }

    public static bool IsRegisterLottery(this string data)
    {
        return data == TelegramMessages.RegisterLottery;
    }

    public static bool IsCallbackQueryNull(this Update update)
    {
        return update.CallbackQuery is null;
    }

    public static bool IsCallbackQueryDataNull(this Update update, out string data)
    {
        if (update.IsCallbackQueryNull())
        {
            data = string.Empty;
            return false;
        }
        
        var query = update.CallbackQuery!;
        if (query.Data is null)
        {
            data = string.Empty;
            return false;
        }

        data = query.Data!;
        return true;
    }

    public static bool IsUserSaveSuccess(this IdentityResult result)
    {
        return result.Succeeded;
    }

    public static bool IsStart(this string? txt)
    {
        return txt == TelegramMessages.Start;
    }
    
    public static bool IsConfirmVoice(this string? txt)
    {
        return txt == TelegramMessages.ConfirmVoice;
    }
    
    public static bool IsPreConfirmVoice(this string? txt)
    {
        return txt == TelegramMessages.PreConfirmVoice;
    }

    public static bool IsTxtNull(this Update update, out string? txt, out Message? msg)
    {
        if (update.Message is null)
        {
            txt = null;
            msg = null;
            return false;
        }

        msg = update.Message!;
        txt = msg.Text;
        
        if (string.IsNullOrEmpty(txt))
        {
            return false;
        }

        return true;
    }

    public static bool IsUserNull(this IdentityUserCustom? user)
    {
        return user is null;
    }

    public static bool IsMessage(this Update update)
    {
        return update.Type == UpdateType.Message;
    }
    
    public static bool IsCallbackQuery(this Update update)
    {
        return update.Type == UpdateType.CallbackQuery;
    }
    
    public static bool IsChatNull(this Chat? chat)
    {
        return chat is null;
    }

    public static bool IsNeedToDoLottery(List<Lottery> queueds, int validMainChannelMessagesCount)
    {
        if (queueds.Count == 0)
        {
            return false;
        }

        if (validMainChannelMessagesCount > 0)
        {
            return false;
        }

        return true;
    }

    public static bool IsChatIdNull(this Lottery lottery)
    {
        if (lottery.User?.ChatId is null)
        {
            return true;
        }

        return false;
    }

    public static bool IsOneQueuedCount(this List<Lottery> queued)
    {
        return queued.Count == 1;
    }
    
    public static readonly Func<Lottery, bool> PossibleWinnerPredicate = x => x.VoiceMessageId != null && x.LotteryDate!.Value.AddMinutes(TelegramMessages.ValidLotteryDateMinute) > DateTime.Now;

    public static readonly Func<Lottery, bool> ExpiredWinnerPredicate = x => x.VoiceMessageId == null && x.LotteryDate!.Value.AddMinutes(TelegramMessages.ValidLotteryDateMinute) < DateTime.Now && x.User != null && x.User.ChatId != null;
}