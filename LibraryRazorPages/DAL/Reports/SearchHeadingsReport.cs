using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models;

namespace LibraryRazorPages.DAL.Reports
{
    public class SearchHeadingsReport : IReportProvider
    {
        private readonly IReportRepository _repo;

        public SearchHeadingsReport(IReportRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public string Key => "search";
        public string Title => "Поиск по заголовкам";
        public Type ModelType => typeof(BookHeading);
        public bool HasParameters => true;

        public async Task<IReadOnlyList<object>> GetDataAsync(IDictionary<string, string> parameters, CancellationToken ct = default)
        {
            if (!parameters.TryGetValue("term", out var term) || string.IsNullOrWhiteSpace(term))
                return Array.Empty<object>();

            var data = await _repo.SearchBooksByHeadingAsync(term);
            return data.Cast<object>().AsList();
        }
    }
}