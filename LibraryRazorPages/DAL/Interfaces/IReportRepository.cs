using LibraryRazorPages.Models;

namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IReportRepository
    {
        Task<IReadOnlyList<BookHeading>> GetBookHeadingsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BookHeadingCount>> GetHeadingCountsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BookFirstHeading>> GetFirstHeadingsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BookHeading>> SearchBooksByHeadingAsync(string term, CancellationToken cancellationToken = default);
    }
}
