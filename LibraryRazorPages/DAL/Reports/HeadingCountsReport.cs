using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models;

namespace LibraryRazorPages.DAL.Reports
{
    public class HeadingCountsReport : IReportProvider
    {
        private readonly IReportRepository _repo;

        public HeadingCountsReport(IReportRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public string Key => "count";
        public string Title => "Количество заголовков";
        public Type ModelType => typeof(BookHeadingCount);
        public bool HasParameters => false;

        public async Task<IReadOnlyList<object>> GetDataAsync(IDictionary<string, string> parameters, CancellationToken ct = default)
        {
            _ = parameters;
            var data = await _repo.GetHeadingCountsAsync();
            return data.Cast<object>().AsList();
        }
    }
}