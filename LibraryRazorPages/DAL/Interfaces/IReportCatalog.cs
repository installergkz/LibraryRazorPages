namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IReportCatalog
    {
        IReadOnlyList<IReportProvider> All { get; }
        IReportProvider? Get(string key);
    }
}
