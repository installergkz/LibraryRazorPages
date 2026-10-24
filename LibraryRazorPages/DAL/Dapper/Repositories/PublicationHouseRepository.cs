using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.Models;

namespace LibraryRazorPages.DAL.Dapper.Repositories
{
    public class PublicationHouseRepository : IEntityRepository<PublicationHouse>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<PublicationHouseRepository> _logger;

        public PublicationHouseRepository(IDbConnectionFactory connectionFactory, ILogger<PublicationHouseRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PublicationHouse?> GetAsync(int id, CancellationToken ct = default)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                return await conn.QueryFirstOrDefaultAsync<PublicationHouse>(Queries.StoredProcedures.GetPublicationHouses, new { Id = id });
            }
            catch (Exception ex)
            {
                var message = $"Не удалось получить издательство с ИД {id}.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<IReadOnlyList<PublicationHouse>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                var pubHouses = await conn.QueryAsync<PublicationHouse>(Queries.StoredProcedures.GetPublicationHouses);
                return pubHouses.AsList();
            }
            catch (Exception ex)
            {
                var message = "Не удалось получить издательства.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<int> CreateAsync(PublicationHouse publicationHouse, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(PublicationHouse publicationHouse, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

    }
}
