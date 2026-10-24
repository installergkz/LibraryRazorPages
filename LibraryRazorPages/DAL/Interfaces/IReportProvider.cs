namespace LibraryRazorPages.DAL.Interfaces
{
    public interface IReportProvider
    {
        string Key { get; }
        string Title { get; }
        Type ModelType { get; }
        bool HasParameters { get; }
        Task<IReadOnlyList<object>> GetDataAsync(IDictionary<string, string> parameters, CancellationToken ct = default);
    }
}
