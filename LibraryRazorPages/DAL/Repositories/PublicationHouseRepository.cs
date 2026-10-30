using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.Models;
using System.Data;

namespace LibraryRazorPages.DAL.Repositories
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

        public async Task<PublicationHouse?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var command = new CommandDefinition
                (
                   commandText: Queries.StoredProcedures.GetPublicationHouses,
                   parameters: new { Id = id },
                   commandType: CommandType.StoredProcedure,
                   cancellationToken: cancellationToken
                );

                using var conn = _connectionFactory.CreateConnection();
                return await conn.QueryFirstOrDefaultAsync<PublicationHouse>(command);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                var message = $"Не удалось получить издательство с ИД {id}.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<IReadOnlyList<PublicationHouse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var command = new CommandDefinition
                (
                   commandText: Queries.StoredProcedures.GetPublicationHouses,
                   commandType: CommandType.StoredProcedure,
                   cancellationToken: cancellationToken
                );

                using var conn = _connectionFactory.CreateConnection();
                var pubHouses = await conn.QueryAsync<PublicationHouse>(command);
                return pubHouses.AsList();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                var message = "Не удалось получить издательства.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<int> CreateAsync(PublicationHouse publicationHouse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(PublicationHouse publicationHouse, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            throw new NotImplementedException();
        }

    }
}
