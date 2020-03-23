using System;
using Lightest.Data.Mongo.Models;
using Lightest.Data.Mongo.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Lightest.Data.Mongo.Services
{
    public class UploadDataService : IUploadDataRepository
    {
        private readonly IMongoCollection<UploadData> _uploadData;

        private readonly MongoDBStoreDatabaseSettings _mongoDBStoreDatabaseSettings;

        protected string CollectionName => _mongoDBStoreDatabaseSettings.UploadDataCollectionName;

        public UploadDataService(IOptionsMonitor<MongoDBStoreDatabaseSettings> optionsMonitor)
        {
            _mongoDBStoreDatabaseSettings = optionsMonitor.CurrentValue;
            var client = new MongoClient(_mongoDBStoreDatabaseSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDBStoreDatabaseSettings.DatabaseName);
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
