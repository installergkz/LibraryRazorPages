using LibraryRazorPages.Enums;

namespace LibraryRazorPages.Services.Interfaces
{
    public interface IMongoLogService
    {
        Task LogAsync(string entityType, string entityId, string entityName, LogAction action, int? userId, CancellationToken ct = default);
    }
}
