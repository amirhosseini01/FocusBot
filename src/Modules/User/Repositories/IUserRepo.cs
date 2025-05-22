using FocusBot.Data.Repositories;
using FocusBot.Modules.User.Models;

namespace FocusBot.Modules.User.Repositories;

public interface IUserRepo : IGenericRepository<IdentityUserCustom>
{
    Task<bool> AnyChatId(long chatId, string username, CancellationToken ct);
}