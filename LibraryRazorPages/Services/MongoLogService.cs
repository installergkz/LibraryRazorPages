using LibraryRazorPages.Enums;
using LibraryRazorPages.Services.Interfaces;
using MongoDB.Driver;

public class MongoLogService : IMongoLogService
{
    private readonly IMongoCollection<ActivityLog> _logsCollection;

    public MongoLogService(IMongoCollection<ActivityLog> logsCollection)
    {
        _logsCollection = logsCollection ?? throw new ArgumentNullException(nameof(logsCollection));
    }

    public async Task LogAsync(string entityType, string entityId, string entityName, LogAction action,
        string? userName, CancellationToken cancellationToken = default)
    {
        var log = new ActivityLog
        {
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Action = action,
            UserName = userName,
            Timestamp = DateTime.UtcNow
        };
        await _logsCollection.InsertOneAsync(log, null, cancellationToken);
    }
}