using LibraryRazorPages.DAL.Helpers;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models;
using LibraryRazorPages.Services.Interfaces;

namespace LibraryRazorPages.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _books;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository books, ILogger<BookService> logger)
        {
            _books = books ?? throw new ArgumentNullException(nameof(books));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Book?> GetAsync(int id, CancellationToken ct = default)
        {
            var book = await _books.GetAsync(id);
            book?.ContentsList = HtmlToXmlConverter.XmlToHtml(book.ContentsList ?? string.Empty);
            return book;
        }

        public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default) => await _books.GetAllAsync();

        public async Task<int> CreateAsync(Book book, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(book);

            if (string.IsNullOrWhiteSpace(book.Name))
                throw new InvalidOperationException("Название книги обязательно.");
            
            if (string.IsNullOrWhiteSpace(book.Authors))
                throw new InvalidOperationException("Авторы обязательны.");

            if (!string.IsNullOrWhiteSpace(book.ISBN))
            {
                var existing = await _books.GetByIsbnAsync(book.ISBN);
                if (existing is not null)
                    throw new InvalidOperationException($"ISBN «{book.ISBN}» уже используется.");
            }

            book.ContentsList = HtmlToXmlConverter.HtmlToWellFormedXml(book.ContentsList ?? string.Empty);

            var id = await _books.CreateAsync(book);
            _logger.LogInformation("Создана книга с ИД {BookId}.", id);
            
            return id;
        }

        public async Task<bool> UpdateAsync(Book book, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(book);

            if (string.IsNullOrWhiteSpace(book.Name))
                throw new InvalidOperationException("Название книги обязательно.");

            if (string.IsNullOrWhiteSpace(book.Authors))
                throw new InvalidOperationException("Авторы обязательны.");

            if (!string.IsNullOrWhiteSpace(book.ISBN))
            {
                var existing = await _books.GetByIsbnAsync(book.ISBN);
                if (existing is not null && existing.Id != book.Id)
                    throw new InvalidOperationException($"ISBN «{book.ISBN}» уже используется.");
            }

            book.ContentsList = HtmlToXmlConverter.HtmlToWellFormedXml(book.ContentsList ?? string.Empty);

            var updated = await _books.UpdateAsync(book);
            if (updated)
                _logger.LogInformation("Обновлена книга с ИД {BookId}.", book.Id);

            return updated;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default) => await _books.DeleteAsync(id);

    }
}