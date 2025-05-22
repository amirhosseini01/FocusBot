using FocusBot.Modules.Telegram.Common;
using FocusBot.Modules.Telegram.Conditions;
using FocusBot.Modules.Telegram.Models;
using FocusBot.Modules.Telegram.Repositories;
using Telegram.Bot;

namespace FocusBot.Modules.Telegram.Services;

public class LotteryBackground(ILotteryRepo lotteryRepo, ITelegramBotClient bot)
{
    public async Task DoLottery()
    {
        var winners = await lotteryRepo.GetWinners();

        var expiredUsers = winners.Where(LotteryConditions.ExpiredWinnerPredicate).ToList();
        if (expiredUsers.Count > 0)
        {
            foreach (var expiredUser in expiredUsers)
            {
                expiredUser.ModifiedDate = DateTime.Now;
                expiredUser.VoiceMessageId = 0;
                expiredUser.ChannelMessageId = 0;
                expiredUser.ArchiveMessageId = 0;
                
                var expiredChat = await bot.GetChat(expiredUser.User!.ChatId!);
                await SendMessages.SendAttemptOnLotteryAgainMessage(bot: bot, chat: expiredChat);
            }

            try
            {
                await lotteryRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        var expiredChannelVoices = await lotteryRepo.GetChannelMessages();
        if (expiredChannelVoices.Messages.Count > 0)
        {
            foreach (var expiredChannelVoice in expiredChannelVoices.Messages)
            {
                if (expiredChannelVoice.ArchiveMessageId > 0)
                {
                    var archiveChannel = await bot.GetChat(TelegramMessages.FocusArchiveChannel);
                    await bot.DeleteMessage(chatId: archiveChannel.Id, expiredChannelVoice.ArchiveMessageId.Value);

                    expiredChannelVoice.DeleteFromArchiveChannelDate = DateTime.Now;
                    expiredChannelVoice.ModifiedDate = DateTime.Now;
                }
                if (expiredChannelVoice.ChannelMessageId > 0 && expiredChannelVoices.MainChannelMessageCount > TelegramMessages.LegalVoiceMessagesInMainChannel)
                {
                    var channel = await bot.GetChat(TelegramMessages.FocusChannel);
                    var userChat = await bot.GetChat(expiredChannelVoice.User!.ChatId!);

                    await HandleTelegramUpdates.SendToArchiveChannel(bot: bot, lotteryRepo: lotteryRepo, chat: userChat, expiredChannelVoice);

                    await bot.DeleteMessage(chatId: channel.Id, expiredChannelVoice.ChannelMessageId.Value);

                    expiredChannelVoice.DeleteFromMainChannelDate = DateTime.Now;
                    expiredChannelVoice.ModifiedDate = DateTime.Now;
                }
            }

            try
            {
                await lotteryRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
            }
        }

        var needToShareVoice = winners.Where(LotteryConditions.PossibleWinnerPredicate).MinBy(x => x.LotteryDate);
        if (needToShareVoice is not null)
        {
            var winnerChat = await bot.GetChat(needToShareVoice.User!.ChatId!);
            var summery = await lotteryRepo.GetSummary(needToShareVoice.UserId);
            if (summery is not null && summery.IsNeedSendToChannels())
            {
                await HandleTelegramUpdates.SendToMainChannel(bot: bot, lotteryRepo: lotteryRepo, chat: winnerChat, lottery: needToShareVoice);
            }
            else if (!needToShareVoice.Confirmed)
            {
                await SendMessages.SendConfirmVoiceMessage(bot: bot, chat: winnerChat);
            }
        }

        var queueds = await lotteryRepo.GetQueueds();
        var validMainChannelMessagesCount = await lotteryRepo.GetValidMainChannelMessagesCount();
        if (!LotteryConditions.IsNeedToDoLottery(queueds, validMainChannelMessagesCount))
        {
            return;
        }

        var winner = SelectPersonAsWinner(queueds);
        if (winner.IsChatIdNull())
        {
            return;
        }

        await UpdateWinner(winner);

        var chat = await bot.GetChat(winner.User!.ChatId!.Value);

        await SendMessages.SendWinnerCongrats(bot: bot, chat: chat);

        if (winner.VoiceMessageId > 0)
        {
            await SendMessages.SendConfirmVoiceMessage(bot: bot, chat: chat);
            return;
        }

        // await HandleTelegramUpdates.SendToMainChannel(bot: bot, lotteryRepo: lotteryRepo, chat: chat, lottery: winner);
    }


    private static Lottery SelectPersonAsWinner(List<Lottery> queued)
    {
        if (queued.IsOneQueuedCount())
        {
            return queued[0];
        }

        var rndNum = GenerateRandomNumber(queued.Count);

        return queued[rndNum];
    }

    private async Task UpdateWinner(Lottery winner)
    {
        winner.LotteryDate = DateTime.Now;
        winner.IsWinner = true;

        await lotteryRepo.SaveChangesAsync();
    }

    private static int GenerateRandomNumber(int max, int min = 0)
    {
        var rnd = new Random();
        return rnd.Next(minValue: min, maxValue: max);
    }
}