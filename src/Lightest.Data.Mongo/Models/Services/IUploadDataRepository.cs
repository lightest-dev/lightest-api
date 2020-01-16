using System;
namespace Lightest.Data.Mongo.Models.Services
{
    public interface IUploadDataRepository
    {
        public UploadData Get(Guid id);
        
        public UploadData Add(UploadData uploadData);
        
        public void Delete(Guid id);
    }
}
