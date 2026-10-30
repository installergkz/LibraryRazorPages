using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.Models;
using System.Data;

namespace LibraryRazorPages.DAL.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(IDbConnectionFactory connectionFactory, ILogger<BookRepository> logger)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Book?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await using var conn = _connectionFactory.CreateConnection();
                var books = await QueryBooksAsync(conn, new { Id = id }, cancellationToken);
                return books.FirstOrDefault();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                var message = $"Не удалось получить книгу с ИД {id}.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await using var conn = _connectionFactory.CreateConnection();
                return await QueryBooksAsync(conn, null, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                var message = "Не удалось получить книги.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return null;

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await using var conn = _connectionFactory.CreateConnection();
                var books = await QueryBooksAsync(conn, new { ISBN = isbn }, cancellationToken);
                return books.FirstOrDefault();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить книгу по ISBN {Isbn}.", isbn);
                throw new RepositoryException("Не удалось получить книгу.", ex);
            }
        }

        public async Task<int> CreateAsync(Book book, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var queryParams = CreateParams(book);
                queryParams.Add("NewBookId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                
                var command = new CommandDefinition(
                    commandText: Queries.StoredProcedures.CreateBook,
                    commandType: CommandType.StoredProcedure,
                    parameters: queryParams,
                    cancellationToken: cancellationToken
                );

                await using var conn = _connectionFactory.CreateConnection();
                await conn.ExecuteAsync(command);
                
                var newBookId = queryParams.Get<int?>("NewBookId");
                if (newBookId is null || newBookId <= 0)
                    throw new InvalidOperationException("Хранимая процедура не вернула корректный Id.");

                return newBookId.Value;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (RepositoryException) { throw; }
            catch (Exception ex)
            {
                var message = "Не удалось создать книгу";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(book);

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var queryParams = CreateParams(book, book.Id);
                var command = new CommandDefinition(
                    commandText: Queries.StoredProcedures.UpdateBook,
                    commandType: CommandType.StoredProcedure,
                    parameters: queryParams,
                    cancellationToken: cancellationToken
                );

                await using var conn = _connectionFactory.CreateConnection();
                var affected = await conn.ExecuteAsync(command);
                if (affected == 0)
                {
                    var message = $"Книга с ИД {book.Id} не найдена.";
                    _logger.LogWarning(message);
                    throw new KeyNotFoundException(message);
                }

                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (KeyNotFoundException) { throw; }
            catch (Exception ex)
            {
                var message = $"Не удалось обновить книгу с ИД {book.Id}.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var command = new CommandDefinition(
                    commandText: Queries.StoredProcedures.DeleteBook,
                    commandType: CommandType.StoredProcedure,
                    parameters: new { Id = id },
                    cancellationToken: cancellationToken
                );

                await using var conn = _connectionFactory.CreateConnection();
                var affected = await conn.ExecuteAsync(command);
                if (affected == 0)
                {
                    var message = $"Книга с ИД {id} не найдена.";
                    _logger.LogWarning(message);
                    throw new KeyNotFoundException(message);
                }

                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
            catch (KeyNotFoundException) { throw; }
            catch (Exception ex)
            {
                var message = $"Не удалось удалить книгу с ИД {id}";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        private static async Task<IReadOnlyList<Book>> QueryBooksAsync(IDbConnection connection,
            object? param = null, CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                commandText: Queries.StoredProcedures.GetBooks,
                commandType: CommandType.StoredProcedure,
                parameters: param,
                cancellationToken: cancellationToken
            );

            var books = await connection.QueryAsync<Book, PublicationHouse, Book>(
                command,
                (book, ph) =>
                {
                    book.PublicationHouse = ph;
                    return book;
                },
                splitOn: "PublicationHouseId");

            return books.AsList();
        }

        private static DynamicParameters CreateParams(Book book, int? id = null)
        {
            var p = new DynamicParameters();
            if (id.HasValue)
                p.Add("Id", id.Value);
            p.Add("Name", book.Name);
            p.Add("Authors", book.Authors);
            p.Add("PublicationYear", book.PublicationYear);
            p.Add("PublicationHouseId", book.PublicationHouse?.Id);
            p.Add("PagesCount", book.PagesCount);
            p.Add("ISBN", book.ISBN);
            p.Add("Note", book.Note);
            p.Add("ContentsList", book.ContentsList);
            return p;
        }

    }
}
