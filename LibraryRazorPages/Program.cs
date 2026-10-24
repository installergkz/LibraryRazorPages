using LibraryRazorPages.DAL.Dapper.Repositories;
using LibraryRazorPages.DAL.Helpers;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.Reports;
using LibraryRazorPages.Models;
using LibraryRazorPages.Services;
using LibraryRazorPages.Services.Interfaces;
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

services.AddRazorPages();

services.AddSingleton<MongoLogService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
