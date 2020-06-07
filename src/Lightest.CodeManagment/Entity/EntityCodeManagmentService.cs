using System;
using Lightest.Data;
using Lightest.Data.CodeManagment.Services;
using Lightest.Data.Models;
using Lightest.CodeManagment.Models;

namespace Lightest.CodeManagment.Entity
{
    public class EntityCodeManagmentService : ICodeManagmentService
    {
        private readonly RelationalDbContext _context;

        public EntityCodeManagmentService(RelationalDbContext context)
        {
            _context = context;
        }

        public UploadData Add(UploadData uploadData)
        {
            if (uploadData.Project != null)
            {
                throw new NotSupportedException("Projects cannot be stored in Relational DB");
            }

            _context.CodeUploads.Add(new CodeUpload
            {
                UploadId = uploadData.Id,
                Code = uploadData.Code
            });

            _context.SaveChanges();

            return uploadData;
        }

        public void Delete(Guid id)
        {
            var upload = _context.CodeUploads.Find(id);
            _context.CodeUploads.Remove(upload);
            _context.SaveChanges();
        }

        public UploadData Get(Guid id)
        {
            var upload = _context.CodeUploads.Find(id);

            var uploadData = new UploadData
            {
                Id = upload.UploadId,
                Code = upload.Code
            };

            return uploadData;
        }
    }
}
