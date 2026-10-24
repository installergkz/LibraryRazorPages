using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.Models.Reports;

namespace LibraryRazorPages.DAL.Dapper.Repositories
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

        public async Task<IReadOnlyList<BookHeading>> GetBookHeadingsAsync(CancellationToken ct = default)
        {
            try
            {
                return await QueryAsync<BookHeading>(Queries.StoredProcedures.GetBookHeadings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось выполнить отчёт {ReportName}.", Queries.StoredProcedures.GetBookHeadings);
                throw new RepositoryException("Не удалось получить заголовки книг.", ex);
            }
        }

        public async Task<IReadOnlyList<BookHeadingCount>> GetHeadingCountsAsync(CancellationToken ct = default)
        {
            try
            {
                return await QueryAsync<BookHeadingCount>(Queries.StoredProcedures.GetHeadingCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось выполнить отчёт {ReportName}.", Queries.StoredProcedures.GetHeadingCounts);
                throw new RepositoryException("Не удалось получить количество заголовков.", ex);
            }
        }

        public async Task<IReadOnlyList<BookFirstHeading>> GetFirstHeadingsAsync(CancellationToken ct = default)
        {
            try
            {
                return await QueryAsync<BookFirstHeading>(Queries.StoredProcedures.GetFirstHeadings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось выполнить отчёт {ReportName}.", Queries.StoredProcedures.GetFirstHeadings);
                throw new RepositoryException("Не удалось получить первые заголовки.", ex);
            }
        }

        public async Task<IReadOnlyList<BookHeading>> SearchBooksByHeadingAsync(string term, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Array.Empty<BookHeading>();

            try
            {
                return await QueryAsync<BookHeading>(Queries.StoredProcedures.SearchBooksByHeading, new { pattern = "%" + term + "%" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось выполнить отчёт {ReportName} с term = {Term}.", Queries.StoredProcedures.SearchBooksByHeading, term);
                throw new RepositoryException("Не удалось найти книги по заголовку.", ex);
            }
        }

        private async Task<IReadOnlyList<T>> QueryAsync<T>(string procedure, object? param = null, CancellationToken ct = default)
        {
            using var conn = _connectionFactory.CreateConnection();
            var data = await conn.QueryAsync<T>(procedure, param);
            return data.AsList();
        }

    }
}
