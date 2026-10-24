using LibraryRazorPages.Models;

namespace LibraryRazorPages.Services.Interfaces
{
    public interface IBookService
    {
        Task<Book?> GetAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default);
        Task<int> CreateAsync(Book book, CancellationToken ct = default);
        Task<bool> UpdateAsync(Book book, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
