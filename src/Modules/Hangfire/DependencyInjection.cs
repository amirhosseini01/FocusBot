using Hangfire;

namespace FocusBot.Modules.Hangfire;

public static class DependencyInjection
{
    public static void AddHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("MainContext")));

        builder.Services.AddHangfireServer();
    }

    public static void UseTheHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard();
    }
}