using FocusBot.Common;
using FocusBot.Data;
using FocusBot.Modules.Hangfire;
using FocusBot.Modules.Telegram;
using FocusBot.Modules.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddWebApp();

builder.AddHangfire();
builder.AddTelegram();
builder.AddUser();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<MainContext>();
    dbContext.Database.Migrate();
}

app.UseTheHangfire();

app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{
}
else
{
    // app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();