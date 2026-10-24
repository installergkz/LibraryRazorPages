using LibraryRazorPages.Models;

namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IBookRepository : IEntityRepository<Book>
    {
        Task<Book?> GetByIsbnAsync(string isbn, CancellationToken ct = default);
    }
}
