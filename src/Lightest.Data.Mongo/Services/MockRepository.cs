using System;
using System.Collections.Generic;
using Lightest.Data.Mongo.Models;

namespace Lightest.Data.Mongo.Services
{
    public class MockRepository : IUploadDataRepository
    {
        private readonly Dictionary<Guid, UploadData> _uploadsDictionary;

        public MockRepository()
        {
            _uploadsDictionary = new Dictionary<Guid, UploadData>();
        }

        public UploadData Add(UploadData uploadData)
        {
            if (uploadData.Id == default)
            {
                uploadData.Id = Guid.NewGuid();
            }

            _uploadsDictionary.Add(uploadData.Id, uploadData);

            return uploadData;
        }

        public void Delete(Guid id) => throw new NotImplementedException();

        public UploadData Get(Guid id)
            => (_uploadsDictionary.TryGetValue(id, out var value)) ? value : null;
    }
}
