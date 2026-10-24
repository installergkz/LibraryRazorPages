using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.DAL.SQL;
using LibraryRazorPages.Models;
using System.Data;

namespace LibraryRazorPages.DAL.Dapper.Repositories
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

        public async Task<Book?> GetAsync(int id, CancellationToken ct = default)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                var books = await QueryBooksAsync(conn, new { Id = id });
                return books.FirstOrDefault();
            }
            catch (Exception ex)
            {
                var message = $"Не удалось получить книгу с ИД {id}.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                return await QueryBooksAsync(conn);
            }
            catch (Exception ex)
            {
                var message = "Не удалось получить книги.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return null;

            try
            {
                using var conn = _connectionFactory.CreateConnection();
                var books = await QueryBooksAsync(conn, new { ISBN = isbn });
                return books.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось получить книгу по ISBN {Isbn}.", isbn);
                throw new RepositoryException("Не удалось получить книгу.", ex);
            }
        }

        public async Task<int> CreateAsync(Book book, CancellationToken ct = default)
        {
            try
            {
                var queryParams = CreateParams(book);
                queryParams.Add("NewBookId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                
                using var conn = _connectionFactory.CreateConnection();
                await conn.ExecuteAsync(Queries.StoredProcedures.CreateBook, queryParams);
                
                var newBookId = queryParams.Get<int?>("NewBookId");
                if (newBookId is null || newBookId <= 0)
                    throw new InvalidOperationException("Хранимая процедура не вернула корректный Id.");

                return newBookId.Value;
            }
            catch (RepositoryException) { throw; }
            catch (Exception ex)
            {
                var message = "Не удалось создать книгу";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<bool> UpdateAsync(Book book, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(book);

            try
            {
                var queryParams = CreateParams(book, book.Id);

                using var conn = _connectionFactory.CreateConnection();
                var affected = await conn.ExecuteAsync(Queries.StoredProcedures.UpdateBook, queryParams);
                if (affected == 0)
                {
                    var message = $"Книга с ИД {book.Id} не найдена.";
                    _logger.LogWarning(message);
                    throw new KeyNotFoundException(message);
                }

                return true;
            }
            catch (KeyNotFoundException) { throw; }
            catch (Exception ex)
            {
                var message = $"Не удалось обновить книгу с ИД {book.Id}.";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            try
            {
                using var conn = _connectionFactory.CreateConnection();
                var affected = await conn.ExecuteAsync(Queries.StoredProcedures.DeleteBook, new { Id = id });
                if (affected == 0)
                {
                    var message = $"Книга с ИД {id} не найдена.";
                    _logger.LogWarning(message);
                    throw new KeyNotFoundException(message);
                }

                return true;
            }
            catch (KeyNotFoundException) { throw; }
            catch (Exception ex)
            {
                var message = $"Не удалось удалить книгу с ИД {id}";
                _logger.LogError(ex, message);
                throw new RepositoryException(message, ex);
            }
        }

        private static async Task<IReadOnlyList<Book>> QueryBooksAsync(IDbConnection connection, object? param = null, CancellationToken ct = default)
        {
            var books = await connection.QueryAsync<Book, PublicationHouse, Book>(
                Queries.StoredProcedures.GetBooks,
                (book, ph) =>
                {
                    book.PublicationHouse = ph;
                    return book;
                },
                splitOn: "PublicationHouseId",
                param: param);

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
