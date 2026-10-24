using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models.Reports;

namespace LibraryRazorPages.DAL.Reports
{
    public class HeadingsReport : IReportProvider
    {
        private readonly IReportRepository _repo;

        public HeadingsReport(IReportRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public string Key => "headings";
        public string Title => "Все заголовки книг";
        public Type ModelType => typeof(BookHeading);
        public bool HasParameters => false;

        public async Task<IReadOnlyList<object>> GetDataAsync(IDictionary<string, string> parameters, CancellationToken ct = default)
        {
            _ = parameters;
            var data = await _repo.GetBookHeadingsAsync();
            return data.Cast<object>().AsList();
        }
        
    }
}