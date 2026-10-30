using LibraryRazorPages.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.String)]
    public LogAction Action { get; set; }
    public int? UserId { get; set; }
    public string? UserLogin { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
