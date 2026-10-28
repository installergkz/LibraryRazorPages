using LibraryRazorPages.DAL.Dapper.Repositories;
using LibraryRazorPages.DAL.Helpers;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.Reports;
using LibraryRazorPages.Models;
using LibraryRazorPages.Services;
using LibraryRazorPages.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var services = builder.Services;

services.AddDbProviderFactory(builder.Configuration);

services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IBookService, BookService>();

services.AddScoped<IEntityRepository<PublicationHouse>, PublicationHouseRepository>();
services.AddScoped<IPublicationHouseService, PublicationHouseService>();

services.AddScoped<IReportRepository, ReportRepository>();
services.AddScoped<IReportProvider, HeadingsReport>();
services.AddScoped<IReportProvider, HeadingCountsReport>();
services.AddScoped<IReportProvider, FirstHeadingsReport>();
services.AddScoped<IReportProvider, SearchHeadingsReport>();

services.AddScoped<IReportCatalog, ReportCatalog>();

services.AddRazorPages(options =>
{
    // Запрещаем кэшировать страницы
    options.Conventions.ConfigureFilter(new Microsoft.AspNetCore.Mvc.ResponseCacheAttribute
    {
        NoStore = true,
        Location = Microsoft.AspNetCore.Mvc.ResponseCacheLocation.None
    });
});

services.AddSingleton<MongoLogService>();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Куда перенаправлять неавторизованных
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Отключаем для работы в Docker без HTTPS
//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.MapStaticAssets();
app.MapRazorPages();//.WithStaticAssets();

app.Run();
