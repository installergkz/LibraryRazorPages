using LibraryRazorPages.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace LibraryRazorPages.DAL.Helpers
{
    public static class ServiceProviderExtensions
    {
        public static void AddDbProviderFactory(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["Database:Provider"] ?? throw new InvalidOperationException("Database:Provider не задан");
            var connectionString = configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("ConnectionStrings:Default не задан");

            DbProviderFactory factory = provider.ToLowerInvariant() switch
            {
                "mssql" => SqlClientFactory.Instance,
                _ => throw new NotSupportedException($"Неизвестный DBProvider: {provider}")
            };

            services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(factory, connectionString));
        }
    }
}
