using MongoDB.Driver;
using LibraryRazorPages.Models;

namespace LibraryRazorPages.Services
{
    public class MongoLogService
    {
        private readonly IMongoCollection<BookViewLog> _logsCollection;

        public MongoLogService(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("LibraryLogs"); 
            _logsCollection = database.GetCollection<BookViewLog>("BookViews"); 
        }

        public async Task LogViewAsync(string bookTitle, string? userName)
        {
            var log = new BookViewLog
            {
                BookTitle = bookTitle,
                UserName = userName
            };
            await _logsCollection.InsertOneAsync(log);
        }
    }
}
