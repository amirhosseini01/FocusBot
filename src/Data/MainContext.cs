using FocusBot.Modules.Telegram.Models;
using FocusBot.Modules.User.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FocusBot.Data;
//  dotnet ef migrations add Init
//  dotnet ef database update
public class MainContext : IdentityDbContext<IdentityUserCustom>
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
    }

    public DbSet<Lottery> Lotteries { get; set; }
}