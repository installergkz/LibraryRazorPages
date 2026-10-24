using LibraryRazorPages.DAL.Interfaces;
using LibraryRazorPages.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryRazorPages.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly IReportCatalog _catalog;

        public IndexModel(IReportCatalog catalog)
        {
            _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        }

        public ReportViewModel ReportViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string? key, string? term)
        {
            ReportViewModel = new()
            {
                ReportsList = _catalog.All,
                SelectedKey = key
            };

            if (!string.IsNullOrEmpty(key))
            {
                var report = _catalog.Get(key);
                if (report is null)
                    return NotFound();

                ReportViewModel.Report = report;

                var parameters = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(term))
                    parameters["term"] = term;
                ReportViewModel.Parameters = parameters;

                try
                {
                    ReportViewModel.Data = await report.GetDataAsync(parameters);
                }
                catch (Exception ex)
                {
                    ReportViewModel.ErrorMessage = $"Не удалось выполнить отчёт: {ex.Message}";
                }
            }

            return Page();
        }

    }
}
