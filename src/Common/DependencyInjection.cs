using FocusBot.Data;
using FocusBot.Modules.User.Models;
using Microsoft.EntityFrameworkCore;

namespace FocusBot.Common;

public static class DependencyInjection
{
    public static void AddWebApp(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
        builder.Services.AddControllers();
        
        builder.Services.AddDbContext<MainContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("MainContext")));
        
        builder.Services.AddDefaultIdentity<IdentityUserCustom>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<MainContext>();
    }
}