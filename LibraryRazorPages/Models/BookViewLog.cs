using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LibraryRazorPages.Models
{
    public class BookViewLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string BookTitle { get; set; } = string.Empty;
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
        public string? UserName { get; set; }
    }
}
