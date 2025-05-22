using FocusBot.Data;
using FocusBot.Data.Repositories;
using FocusBot.Modules.User.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusBot.Modules.User.Repositories;

public class UserRepo(MainContext context) : GenericRepository<IdentityUserCustom>(context), IUserRepo
{
    private readonly DbSet<IdentityUserCustom> _store = context.Users;

    public async Task<bool> AnyChatId(long chatId, string username, CancellationToken ct)
    {
        return await _store.AnyAsync(x => x.ChatId == chatId && x.UserName == username);
    }
}