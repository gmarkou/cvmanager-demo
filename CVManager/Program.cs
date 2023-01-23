using System.Security.Authentication.ExtendedProtection;
using CVManager.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using CVManager.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Auth/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        option.AccessDeniedPath = new PathString("/Auth/AccessDenied");
    });
builder.Services.AddAntiforgery();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Editor", policy=> policy.RequireClaim("Editor"));
});
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureUoW();
builder.Services.ConfigureFileManager();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCustom404();

Directory.CreateDirectory(Path.Join(app.Configuration.GetValue<string>(WebHostDefaults.ContentRootKey), app.Configuration["FileManagerConfig:path"]));
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();