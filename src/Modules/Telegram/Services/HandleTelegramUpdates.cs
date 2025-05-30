using System.Runtime.InteropServices.ObjectiveC;
using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Conditions;
using FocusBot.Modules.Telegram.Dto;
using FocusBot.Modules.Telegram.Models;
using FocusBot.Modules.Telegram.Repositories;
using FocusBot.Modules.User.Models;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FocusBot.Modules.Telegram.Services;

public class HandleTelegramUpdates(ILotteryRepo lotteryRepo, UserManager<IdentityUserCustom> userManager)
{
    public async Task Handle(ITelegramBotClient bot, Update update, CancellationToken ct = default)
    {
        var chat = GetChat(update);
        if (chat.IsChatNull())
        {
            return;
        }

        var user = await GetUser(chat: chat!);
        if (user.IsUserNull())
        {
            return;
        }

        var lotterySummary = (await lotteryRepo.GetSummary(user!.Id)) ?? new LotterySummaryDto();

        if (update.IsMessage())
        {
            await HandleMessage(bot: bot, update: update, chat: chat!, user: user, lotterySummary: lotterySummary, ct: ct);
            return;
        }


        if (update.IsCallbackQuery())
        {
            await HandleCallbackQuery(bot: bot, update: update, chat: chat!, user: user, lotterySummary: lotterySummary, ct: ct);
            // return;
        }

        // test
    }

    private static Chat? GetChat(Update update)
    {
        if (update.IsMessage())
        {
            return update.Message!.Chat;
        }

        if (update.IsCallbackQuery())
        {
            return update.CallbackQuery!.Message!.Chat;
        }

        return null;
    }

    private async Task<IdentityUserCustom?> GetUser(Chat chat)
    {
        var userName = chat.Username;
        if (string.IsNullOrEmpty(userName))
        {
            userName = chat.FirstName;
        }
        else if (string.IsNullOrEmpty(userName))
        {
            userName = chat.Id.ToString();
        }
        var user = await userManager.FindByNameAsync(userName);
        if (!user.IsUserNull())
        {
            return user;
        }

        user = new IdentityUserCustom
        {
            ChatId = chat.Id,
            UserName = userName,
        };

        var res = await userManager.CreateAsync(user);
        if (!res.IsUserSaveSuccess())
        {
            return null;
        }

        return user;
    }

    private async Task HandleMessage(ITelegramBotClient bot, Update update, Chat chat, LotterySummaryDto lotterySummary, IdentityUserCustom user, CancellationToken ct = default)
    {
        if (update.IsTxtNull(out var txt, out var msg))
        {
            await HandleText(bot: bot, chat: chat, update: update, lotterySummary: lotterySummary, ct: ct);
            return;
        }

        if (lotterySummary.IsHandleVoiceAllowed(msg: msg!))
        {
            await HandleVoice(bot: bot, chat: chat, update: update, user: user, lotterySummary, ct: ct);
        }
    }

    private async Task HandleText(ITelegramBotClient bot, Chat chat, Update update, LotterySummaryDto lotterySummary, CancellationToken ct = default)
    {
        var msg = update.Message!;
        var txt = msg.Text;
        if (txt.IsStart())
        {
            if (lotterySummary.CurrentUserLottery is not null && !lotterySummary.IsCanRegisterAgain())
            {
                await SendMessages.SendAlreadyRegisteredMessage(bot: bot, chat: chat, lotterySummary: lotterySummary, ct: ct);
                return;
            }
            
            await SendMessages.SendStartMessage(bot: bot, chat: chat, ct: ct);
            return;
        }

        if (txt == TelegramMessages.Cancel)
        {
            if (lotterySummary.CurrentUserLottery is null)
            {
                await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
                return;
            }
            
            var currentLottery = lotterySummary.CurrentUserLottery!;
            ReplyMarkup replyMarkup = new InlineKeyboardButton[] { TelegramMessages.LeftFromLottery };
            if (currentLottery.ChannelMessageId != null&& currentLottery.SendToMainChannelDate != null && currentLottery.ArchiveMessageId != null&& currentLottery.SendToArchiveChannelDate != null)
            {
                replyMarkup =  new InlineKeyboardButton[] {TelegramMessages.LeftFromLottery, TelegramMessages.DeleteFromAllChannel};
            }
            else if (currentLottery.ChannelMessageId != null && currentLottery.SendToMainChannelDate != null)
            {
                replyMarkup =  new InlineKeyboardButton[] {TelegramMessages.LeftFromLottery, TelegramMessages.DeleteFromMainChannel};
            }
            else if (currentLottery.ArchiveMessageId != null && currentLottery.SendToArchiveChannelDate != null)
            {
                replyMarkup =  new InlineKeyboardButton[] {TelegramMessages.LeftFromLottery, TelegramMessages.DeleteFromArchiveChannel};
            }

            await SendMessages.SendCancelOptionsMessage(bot: bot, chat: chat, replyMarkup: replyMarkup, ct: ct);
            return;
        }

        if (lotterySummary.IsOnlyVoiceAllowed(msg))
        {
            await SendMessages.SendVoiceOnlyMessage(bot: bot, chat: chat, ct: ct);
            // return;
        }
    }

    private async Task HideVoice(ITelegramBotClient bot, IdentityUserCustom user, Chat chat, CancellationToken ct = default)
    {
        user.HideVoice = true;
        var res = await userManager.UpdateAsync(user);
        if (!res.IsUserSaveSuccess())
        {
            await SendMessages.SendSettingUpdateFailedMessage(bot: bot, chat: chat, ct: ct);
            return;
        }

        await SendMessages.SendSettingUpdateSuccessMessage(bot: bot, chat: chat, ct: ct);
    }

    private async Task HideUserName(ITelegramBotClient bot, IdentityUserCustom user, Chat chat, CancellationToken ct = default)
    {
        user.HideUserName = true;
        var res = await userManager.UpdateAsync(user);
        if (!res.IsUserSaveSuccess())
        {
            await SendMessages.SendSettingUpdateFailedMessage(bot: bot, chat: chat, ct: ct);
            return;
        }

        await SendMessages.SendSettingUpdateSuccessMessage(bot: bot, chat: chat, ct: ct);
    }

    private async Task HandleVoice(ITelegramBotClient bot, Chat chat, Update update, IdentityUserCustom user, LotterySummaryDto lotterySummary, CancellationToken ct = default)
    {
        var msg = update.Message!;

        var currentLottery = lotterySummary.CurrentUserLottery!;
        if (currentLottery.IsLotteryDateExpired())
        {
            await SendMessages.SendAttemptOnLotteryAgainMessage(bot: bot, chat: chat, ct: ct);
            return;
        }
        
        if (msg.IsForwarded())
        {
            await SendMessages.SendCouldNotForwardMessage(bot: bot, chat: chat, ct: ct);
            return;
        }
        
        var voice = msg.Voice!;
        // message is audio
        if (voice.IsInValidDuration())
        {
            await SendMessages.SendInvalidAudioDurationMessage(bot: bot, chat: chat, ct: ct);
            return;
        }

        if (user.IsHideVoice())
        {
            await HandleHideVoice(bot: bot, chat: chat, update: update, lotterySummary: lotterySummary, ct: ct);
        }
        else
        {
            await HandleSimpleVoice(update: update, lotterySummary: lotterySummary, ct: ct);
        }

        if (lotterySummary.IsNeedSendToChannels())
        {
            await SendMessages.SendConfirmVoiceMessage(bot: bot, chat: chat, ct: ct);
        }
        else if(lotterySummary.IsLotteryDateExpired())
        {
            await SendMessages.SendAttemptOnLotteryAgainMessage(bot: bot, chat: chat, ct: ct);
        }
        else if (lotterySummary.CurrentUserLottery?.Confirmed == false)
        {
            await SendMessages.SendPreConfirmVoiceMessage(bot: bot, chat: chat, ct: ct);
        }
    }

    private async Task HandleSimpleVoice(Update update, LotterySummaryDto lotterySummary, CancellationToken ct = default)
    {
        var msg = update.Message!;
        var currentLottery = lotterySummary.CurrentUserLottery!;

        currentLottery.VoiceMessageId = msg.MessageId;
        currentLottery.ModifiedDate = DateTime.Now;
        await lotteryRepo.SaveChangesAsync(ct);
    }

    private async Task HandleHideVoice(ITelegramBotClient bot, Chat chat, Update update, LotterySummaryDto lotterySummary, CancellationToken ct = default)
    {
        var msg = update.Message!;
        var voice = msg.Voice!;
        var fileId = voice.FileId;
        var currentLottery = lotterySummary.CurrentUserLottery!;
        var tgFile = await bot.GetFile(fileId, cancellationToken: ct);

        var filePath = $"/voices/{Guid.NewGuid().ToString()}.ogg";
        var systemPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        
        await using (var stream = File.Create($"{systemPath}{filePath}"))
        {
            await bot.DownloadFile(tgFile, stream, ct);    
        }
        

        currentLottery.FilePath = filePath;
        currentLottery.FileId = fileId;
        currentLottery.VoiceMessageId = msg.MessageId;
        currentLottery.ModifiedDate = DateTime.Now;
        await lotteryRepo.SaveChangesAsync(ct);
        
        if (!string.IsNullOrEmpty(currentLottery.FileId))
        {
            File.Delete($"{systemPath}{currentLottery.FilePath!}");
        }

        await SendVoiceSave(bot, chat, currentLottery, ct);
    }

    private static async Task SendVoiceSave(ITelegramBotClient bot, Chat chat, Lottery currentLottery, CancellationToken ct)
    {
        if (currentLottery.IsVoiceShared(out _))
        {
            await SendMessages.VoiceSavedSuccessFully(bot: bot, chat: chat, ct: ct);
            return;
        }

        await SendMessages.VoiceReplacedSuccessFully(bot: bot, chat: chat, ct: ct);
    }

    public static async Task SendToMainChannel(ITelegramBotClient bot, ILotteryRepo lotteryRepo, Chat chat, Lottery lottery, CancellationToken ct = default)
    {
        if (!lottery.IsVoiceShared(out var voiceMessageId))
        {
            await SendMessages.SendVoiceOnlyMessage(bot: bot, chat: chat, ct: ct);
            return;
        }

        var mainChannel = await bot.GetChat(TelegramMessages.FocusChannel, ct);

        var channelMessage = await bot.ForwardMessage(chatId: mainChannel.Id, fromChatId: chat.Id, messageId: voiceMessageId!.Value, cancellationToken: ct, 
            protectContent: true // remove this line
            );

        lottery.ChannelMessageId = channelMessage.Id;
        lottery.ModifiedDate = DateTime.Now;
        lottery.SendToMainChannelDate = DateTime.Now;

        await lotteryRepo.SaveChangesAsync(ct);

        await SendMessages.SendVoiceHasSharedMessage(bot: bot, chat: chat, channel: TelegramMessages.FocusChannelName, ct: ct);
    }
    
    public static async Task SendToArchiveChannel(ITelegramBotClient bot, ILotteryRepo lotteryRepo, Chat chat, Lottery lottery, CancellationToken ct = default)
    {
        if (!lottery.IsVoiceShared(out var voiceMessageId))
        {
            await SendMessages.SendVoiceOnlyMessage(bot: bot, chat: chat, ct: ct);
            return;
        }

        var archiveChannel = await bot.GetChat(TelegramMessages.FocusArchiveChannel, ct);

        var channelMessage = await bot.ForwardMessage(chatId: archiveChannel.Id, fromChatId: chat.Id, messageId: voiceMessageId!.Value, cancellationToken: ct);

        lottery.ArchiveMessageId = channelMessage.Id;
        lottery.ModifiedDate = DateTime.Now;
        lottery.SendToArchiveChannelDate = DateTime.Now;

        await lotteryRepo.SaveChangesAsync(ct);

        await SendMessages.SendVoiceHasSharedMessage(bot: bot, chat: chat, channel: TelegramMessages.FocusArchiveChannelName, ct: ct);
    }

    private async Task HandleCallbackQuery(ITelegramBotClient bot, Update update, Chat chat, LotterySummaryDto lotterySummary, IdentityUserCustom user, CancellationToken ct = default)
    {
        if (!update.IsCallbackQueryDataNull(out var data))
        {
            return;
        }

        if (data.IsRegisterLottery())
        {
            await RegisterLottery(bot: bot, chat: chat, user: user, update: update, lotterySummary: lotterySummary, ct: ct);
        }
        
        if (data.IsConfirmVoice())
        {
            lotterySummary.CurrentUserLottery!.Confirmed = true;
            lotterySummary.CurrentUserLottery.ModifiedDate = DateTime.Now;
            await lotteryRepo.SaveChangesAsync(ct);
            await ShareIfNeeded(bot, chat, lotterySummary, ct);

            return;
        }

        if (data.IsPreConfirmVoice())
        {
            lotterySummary.CurrentUserLottery!.Confirmed = true;
            lotterySummary.CurrentUserLottery.ModifiedDate = DateTime.Now;
            await lotteryRepo.SaveChangesAsync(ct);
            await ShareIfNeeded(bot, chat, lotterySummary, ct);
            return;
        }

        if (data.IsHideVoice())
        {
            await HideVoice(bot: bot, user: user, chat: chat, ct: ct);
            return;
        }

        if (data.IsHideUserName())
        {
            await HideUserName(bot: bot, user: user, chat: chat, ct: ct);
            // return;
        }
        
        if (data == TelegramMessages.LeftFromLottery)
        {
            if (lotterySummary.CurrentUserLottery is null)
            {
                await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
                return;
            }

            var currentLottery = lotterySummary.CurrentUserLottery!;
            currentLottery.ModifiedDate = DateTime.Now;

            currentLottery.LotteryDate = DateTime.Now.AddDays(-1);
            await lotteryRepo.SaveChangesAsync(ct);
            
            await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
            return;
        }
        
        if (data == TelegramMessages.DeleteFromArchiveChannel)
        {
            if (lotterySummary.CurrentUserLottery is null)
            {
                await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
                return;
            }

            var currentLottery = lotterySummary.CurrentUserLottery!;
            currentLottery.ModifiedDate = DateTime.Now;
            
            if (currentLottery.ArchiveMessageId != null && currentLottery.SendToArchiveChannelDate != null)
            {
                var archiveChannel = await bot.GetChat(TelegramMessages.FocusArchiveChannel, cancellationToken: ct);
                await bot.DeleteMessage(chatId: archiveChannel.Id, currentLottery.ArchiveMessageId.Value, cancellationToken: ct);
                currentLottery.DeleteFromArchiveChannelDate = DateTime.Now;
            }

            currentLottery.LotteryDate = DateTime.Now.AddDays(-1);
            await lotteryRepo.SaveChangesAsync(ct);
            
            await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
            return;
        }
        
        if (data == TelegramMessages.DeleteFromMainChannel)
        {
            if (lotterySummary.CurrentUserLottery is null)
            {
                await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
                return;
            }

            var currentLottery = lotterySummary.CurrentUserLottery!;
            currentLottery.ModifiedDate = DateTime.Now;
            
            if (currentLottery.ChannelMessageId != null && currentLottery.SendToMainChannelDate != null)
            {
                var channel = await bot.GetChat(TelegramMessages.FocusChannel, cancellationToken: ct);
                await bot.DeleteMessage(chatId: channel.Id, currentLottery.ChannelMessageId.Value, cancellationToken: ct);
                currentLottery.DeleteFromMainChannelDate = DateTime.Now;
                currentLottery.ModifiedDate = DateTime.Now;
            }

            currentLottery.LotteryDate = DateTime.Now.AddDays(-1);
            await lotteryRepo.SaveChangesAsync(ct);
            
            await SendMessages.SendSuccessCancelMessage(bot: bot, chat: chat, ct: ct);
            return;
        }
    }

    private async Task<bool> ShareIfNeeded(ITelegramBotClient bot, Chat chat, LotterySummaryDto lotterySummary, CancellationToken ct)
    {
        if (!lotterySummary.IsNeedSendToChannels())
        {
            if (lotterySummary.CurrentUserLottery is null)
            {
                await SendMessages.SendStartMessage(bot: bot, chat: chat, ct: ct);
                return true;
            }

            if (lotterySummary.CurrentUserLottery.LotteryDate is null)
            {
                await SendMessages.SendStartMessage(bot: bot, chat: chat, ct: ct);
                return true;
            }

            if (lotterySummary.CurrentUserLottery.LotteryDate.Value.AddMinutes(TelegramMessages.ValidLotteryDateMinute) < DateTime.Now)
            {
                await SendMessages.SendStartMessage(bot: bot, chat: chat, ct: ct);
                return true;
            }
                
            if (!lotterySummary.IsCurrentUserWinner)
            {
                await SendMessages.SendAlreadyRegisteredMessage(bot: bot, chat: chat, lotterySummary: lotterySummary, ct: ct);
                return true;
            }

            if (!lotterySummary.CurrentUserLottery.Confirmed)
            {
                await SendMessages.SendConfirmVoiceMessage(bot: bot, chat: chat, ct: ct);
                return true;
            }

            if (lotterySummary.MainChannelMessagesCount > 0 || lotterySummary.WinnerCount == 0)
            {
                await SendMessages.SendWaitForMessageMessage(bot: bot, chat: chat, ct: ct);
                return true;
            }
                
            await SendMessages.SendCantConfirmVoiceMessage(bot: bot, chat: chat, ct: ct);
            return true;
        }

        RecurringJob.TriggerJob(TelegramMessages.RecurringJobId);
        // await SendToMainChannel(bot: bot, lotteryRepo: lotteryRepo, chat: chat, lottery: lotterySummary.CurrentUserLottery!, ct: ct);
        return false;
    }

    private async Task AddLottery(Lottery lottery, CancellationToken ct = default)
    {
        await lotteryRepo.AddAsync(lottery, ct);
        await lotteryRepo.SaveChangesAsync(ct);
    }

    private async Task RegisterLottery(ITelegramBotClient bot, Update update, Chat chat, LotterySummaryDto lotterySummary, IdentityUserCustom user, CancellationToken ct = default)
    {
        if (!lotterySummary.IsCanRegisterAgain())
        {
            await SendMessages.SendAlreadyRegisteredMessage(bot: bot, chat: chat, lotterySummary: lotterySummary, ct: ct);
            return;
        }

        var lottery = new Lottery()
        {
            UserId = user.Id,
            RequestDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        await AddLottery(lottery: lottery, ct: ct);

        await SendMessages.SendLotteryRegisteredMessage(bot: bot, update: update, chat: chat, lotterySummary: lotterySummary, ct: ct);

        if (lotterySummary.IsRegisteredUserCouldBeWinner()) 
        {
            lottery.LotteryDate = DateTime.Now;
            lottery.IsWinner = true;
        
            await lotteryRepo.SaveChangesAsync(ct);
        
            await SendMessages.SendWinnerCongrats(bot: bot, chat: chat, ct: ct);
        }
    }
}