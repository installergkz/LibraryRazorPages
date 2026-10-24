using LibraryRazorPages.DAL.Interfaces;

namespace LibraryRazorPages.DAL.Reports
{
    public sealed class ReportCatalog : IReportCatalog
    {
        private readonly IReadOnlyList<IReportProvider> _reports;

        public ReportCatalog(IEnumerable<IReportProvider> reports)
        {
            _reports = reports.ToList();
        }

        public IReadOnlyList<IReportProvider> All => _reports;

        public IReportProvider? Get(string key) => _reports.FirstOrDefault(r => r.Key == key);
    }
}
