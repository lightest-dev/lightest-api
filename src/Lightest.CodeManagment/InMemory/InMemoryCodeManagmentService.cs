using System;
using System.Collections.Generic;
using Lightest.Data.CodeManagment.Services;
using Lightest.CodeManagment.Models;

namespace Lightest.Data.CodeManagment.InMemory
{
    public class InMemoryCodeManagmentService : ICodeManagmentService
    {
        private readonly Dictionary<Guid, UploadData> _uploadsDictionary;

        public InMemoryCodeManagmentService()
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

        public void Delete(Guid id) => _uploadsDictionary.Remove(id);

        public UploadData Get(Guid id)
            => (_uploadsDictionary.TryGetValue(id, out var value)) ? value : null;
    }
}
