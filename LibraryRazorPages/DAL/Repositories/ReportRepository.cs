using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.Models;
using System.Data;

namespace LibraryRazorPages.DAL.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(IDbConnectionFactory connectionFactory, ILogger<ReportRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<BookHeading>> GetBookHeadingsAsync(CancellationToken cancellationToken = default)
        {
            return await QueryAsync<BookHeading>(Queries.StoredProcedures.GetBookHeadings, null, cancellationToken);
        }

        public async Task<IReadOnlyList<BookHeadingCount>> GetHeadingCountsAsync(CancellationToken cancellationToken = default)
        {
            return await QueryAsync<BookHeadingCount>(Queries.StoredProcedures.GetHeadingCounts, null, cancellationToken);
        }

        public async Task<IReadOnlyList<BookFirstHeading>> GetFirstHeadingsAsync(CancellationToken cancellationToken = default)
        {
            return await QueryAsync<BookFirstHeading>(Queries.StoredProcedures.GetFirstHeadings, null, cancellationToken);
        }

        public async Task<IReadOnlyList<BookHeading>> SearchBooksByHeadingAsync(string term, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Array.Empty<BookHeading>();

            return await QueryAsync<BookHeading>(Queries.StoredProcedures.SearchBooksByHeading, new { pattern = "%" + term + "%" }, cancellationToken);
        }

        private async Task<IReadOnlyList<T>> QueryAsync<T>(string procedure, object? param = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var command = new CommandDefinition
                (
                    commandText: procedure,
                    parameters: param,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                );

                await using var conn = _connectionFactory.CreateConnection();
                var data = await conn.QueryAsync<T>(command);
                return data.AsList();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось выполнить хранимую процедуру {procedure}.", procedure);
                throw new RepositoryException($"Не удалось выполнить хранимую процедуру {procedure}.", ex);
            }
        }

    }
}
