using MongoDB.Driver;
using Lightest.Data.Models;

namespace Lightest.Data
{
    public class CodeStorageContext
    {
        private readonly IMongoDatabase _database;

        public CodeStorageContext(IMongoClient client, string dbName)
        {
            if (client != null)
            {
                _database = client.GetDatabase(dbName);
            }
        }

        public IMongoCollection<CodeUpload> CodeUploads => _database.GetCollection<CodeUpload>("code_uploads");

        public IMongoCollection<ArchiveUpload> Archives => _database.GetCollection<ArchiveUpload>("archive_uploads");
    }
}