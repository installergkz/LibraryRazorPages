using LibraryRazorPages.DAL.Interfaces;

namespace LibraryRazorPages.Models.ViewModels
{
    public class ReportViewModel
    {
        public IReportProvider? Report { get; set; }
        public IReadOnlyList<object> Data { get; set; } = Array.Empty<object>();
        public string? SelectedKey { get; set; }
        public IReadOnlyList<IReportProvider> ReportsList { get; set; } = Array.Empty<IReportProvider>();
        public Dictionary<string, string> Parameters { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}