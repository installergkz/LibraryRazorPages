using System.Data.Common;
using LibraryRazorPages.DAL.Interfaces;

namespace LibraryRazorPages.DAL
{
    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;

        public DbConnectionFactory(DbProviderFactory factory, string connectionString)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public DbConnection CreateConnection()
        {
            var conn = _factory.CreateConnection() ?? throw new InvalidOperationException("DbProviderFactory вернул null");
            conn.ConnectionString = _connectionString;
            return conn;
        }
    }
}
