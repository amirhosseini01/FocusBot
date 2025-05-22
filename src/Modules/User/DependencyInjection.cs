using FocusBot.Modules.User.Repositories;

namespace FocusBot.Modules.User;

public static class DependencyInjection
{
    public static void AddUser(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserRepo, UserRepo>();

    }
}