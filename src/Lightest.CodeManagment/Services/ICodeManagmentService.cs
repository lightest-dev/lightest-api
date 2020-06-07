using System;
using Lightest.CodeManagment.Models;

namespace Lightest.Data.CodeManagment.Services
{
    public interface ICodeManagmentService
    {
        UploadData Get(Guid id);

        UploadData Add(UploadData uploadData);

        void Delete(Guid id);
    }
}
