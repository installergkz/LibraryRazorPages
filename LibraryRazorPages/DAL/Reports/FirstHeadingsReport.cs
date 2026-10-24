using Dapper;
using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models.Reports;

namespace LibraryRazorPages.DAL.Reports
{
    public class FirstHeadingsReport : IReportProvider
    {
        private readonly IReportRepository _repo;

        public FirstHeadingsReport(IReportRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public string Key => "first";
        public string Title => "Первый заголовок";
        public Type ModelType => typeof(BookFirstHeading);
        public bool HasParameters => false;

        public async Task<IReadOnlyList<object>> GetDataAsync(IDictionary<string, string> parameters, CancellationToken ct = default)
        {
            _ = parameters;
            var data = await _repo.GetFirstHeadingsAsync();
            return data.Cast<object>().AsList();
        }
    }
}