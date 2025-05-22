using System.ComponentModel.DataAnnotations.Schema;
using FocusBot.Modules.Telegram.Models;
using Microsoft.AspNetCore.Identity;

namespace FocusBot.Modules.User.Models;

public class IdentityUserCustom: IdentityUser, ITelegramInfo
{
    public bool HideUserName { get; set; }
    public bool HideVoice { get; set; }
    public long? ChatId { get; set; }

    [InverseProperty(nameof(Lottery.User))]
    public ICollection<Lottery> Lotteries { get; set; }
}