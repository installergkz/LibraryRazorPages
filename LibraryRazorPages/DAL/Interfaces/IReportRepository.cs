using LibraryRazorPages.Models.Reports;

namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IReportRepository
    {
        Task<IReadOnlyList<BookHeading>> GetBookHeadingsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<BookHeadingCount>> GetHeadingCountsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<BookFirstHeading>> GetFirstHeadingsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<BookHeading>> SearchBooksByHeadingAsync(string term, CancellationToken ct = default);
    }
}
