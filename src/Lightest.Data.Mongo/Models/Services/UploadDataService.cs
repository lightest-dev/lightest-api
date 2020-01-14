using System;
using Lightest.Data.Mongo.Models.Options;
using MongoDB.Driver;

namespace Lightest.Data.Mongo.Models.Services
{
    public class UploadDataService
    {
        private readonly IMongoCollection<UploadData> _uploadData;
        
        protected string CollectionName => nameof(UploadData);
        
        public UploadDataService(MongoDBStoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _uploadData = database.GetCollection<UploadData>(CollectionName);
        }
        
        public UploadData Get(Guid id) =>
            _uploadData.Find<UploadData>(uploadData => uploadData.Id == id).FirstOrDefault();

        public UploadData Add(UploadData uploadData)
        {
            _uploadData.InsertOne(uploadData);
            return uploadData;
        }
        
        public void Remove(Guid id) => 
            _uploadData.DeleteOne(uploadData => uploadData.Id == id);

    }
}
