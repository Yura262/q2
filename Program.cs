using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Черга.Data;
using Черга.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.LoginPath = "/Identity/Account/Login"; // Redirect here if the user is not authenticated
//        options.Cookie.SameSite = SameSiteMode.None;

//        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
//        ////options.Cookie.
//    });
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login"; // Redirect for unauthenticated users
                                                       //options.LogoutPath = "/Identity/Account/Logout"; // Redirect after logout
                                                       //options.Cookie.SameSite = SameSiteMode.None; // Lax allows cookies for same-site requests
                                                       //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Adjust based on HTTP/HTTPS
                                                       //options.Cookie.Domain = null;
                                                       //                                                         //options.Cookie.HttpOnly = true;

    });

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 3;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@<>абвгґдежзийїклмнопрстуфхцчшщьюяАБВГҐДЕЖЗИЙЇКЛМНОПРСТУФХЦЧШЩЬЮЯ";//
    options.User.RequireUniqueEmail = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//app.UseCookiePolicy(new CookiePolicyOptions
//{
//    Secure = CookieSecurePolicy.Always
//});
//var allowedOrigins = new[] { "localhost:44379", "localhost:3000" }; // Ideally comes from appsettings

//app.UseCors(builder =>
//    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().Build());


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
