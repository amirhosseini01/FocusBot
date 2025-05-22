namespace FocusBot.Modules.User.Models;

public interface ITelegramInfo
{
    public bool HideUserName { get; set; }
    public bool HideVoice { get; set; }
    public long? ChatId { get; set; }
}