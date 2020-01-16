using System;
using Lightest.Data.Mongo.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Lightest.Data.Mongo.Models.Services
{
    public class UploadDataService : IUploadDataRepository
    {
        private readonly IMongoCollection<UploadData> _uploadData;
        
        protected string CollectionName => nameof(UploadData);

        private readonly MongoDBStoreDatabaseSettings _mongoDBStoreDatabaseSettings;
        
        IMongoDatabase database;
        public UploadDataService(IOptionsMonitor<MongoDBStoreDatabaseSettings> optionsMonitor)
        {
            _mongoDBStoreDatabaseSettings = optionsMonitor.CurrentValue;
            _uploadData = database.GetCollection<UploadData>(CollectionName);
        }
        
        public UploadData Get(Guid id) =>
            _uploadData.Find<UploadData>(uploadData => uploadData.Id == id).FirstOrDefault();

        public UploadData Add(UploadData uploadData)
        {
            _uploadData.InsertOne(uploadData);
            return uploadData;
        }
        
        public void Delete(Guid id) => 
            _uploadData.DeleteOne(uploadData => uploadData.Id == id);

    }
}
