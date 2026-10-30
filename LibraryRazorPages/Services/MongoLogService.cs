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
        int? userId, CancellationToken cancellationToken = default)
    {
        var log = new ActivityLog
        {
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Action = action,
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };
        await _logsCollection.InsertOneAsync(log, null, cancellationToken);
    }
}