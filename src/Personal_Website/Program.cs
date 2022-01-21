using Microsoft.AspNetCore.Authentication.Cookies;
using Personal_Website.Utilities;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<IViewRenderService, ViewRenderService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(option =>
{
    option.LoginPath = "/Home/Login";
    option.LogoutPath = "/Home/Logout";
    option.ExpireTimeSpan = TimeSpan.FromDays(10);
    option.SlidingExpiration = true;
});

// Create database
Personal_Website.Utilities.Database db = new Personal_Website.Utilities.Database();
if (!db.IsExistDatabase())
    db.CreateDatabase();

// -----------------------------------------------------------------------


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/NotFound/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/admin"))
    {
        string cookies = context.User.FindFirstValue("IsAdmin");
        if (cookies == null)
        {
            context.Response.Redirect("/Home/Login");
        }
        else if (!bool.Parse(cookies))
        {
            context.Response.Redirect("/Home/Login");
        }
    }
    await next.Invoke();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
