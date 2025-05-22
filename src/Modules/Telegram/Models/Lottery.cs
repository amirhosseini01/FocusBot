using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FocusBot.Data.Models;
using FocusBot.Modules.User.Models;
using Telegram.Bot.Types;

namespace FocusBot.Modules.Telegram.Models;

public class Lottery: BaseEntity
{
    [StringLength(450)]
    public required string UserId { get; set; }
    public required DateTime RequestDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime? LotteryDate { get; set; }
    public bool IsWinner { get; set; }
    public DateTime? SendVoiceDate { get; set; }
    public DateTime? SendToMainChannelDate { get; set; }
    public DateTime? SendToArchiveChannelDate { get; set; }
    public DateTime? DeleteFromMainChannelDate { get; set; }
    public DateTime? DeleteFromArchiveChannelDate { get; set; }

    [StringLength(500)]
    public string? FileId { get; set; }
    
    [StringLength(500)]
    public string? FilePath { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public IdentityUserCustom? User { get; set; }

    public int? VoiceMessageId { get; set; }
    public int? ChannelMessageId { get; set; }
    public int? ArchiveMessageId { get; set; }
    public bool Confirmed { get; set; }
}