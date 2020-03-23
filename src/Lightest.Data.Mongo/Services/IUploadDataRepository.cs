using System;
using Lightest.Data.Mongo.Models;

namespace Lightest.Data.Mongo.Services
{
    public interface IUploadDataRepository
    {
        UploadData Get(Guid id);

        UploadData Add(UploadData uploadData);

        void Delete(Guid id);
    }
}
