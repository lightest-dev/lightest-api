using System;

namespace Lightest.Data.Mongo.Models
{
    public class UploadData
    {
        public Guid Id { get; set; }

        public Guid BsonId { get; set; }

        public string Code { get; set; }

        public byte[] Project { get; set; }
    }
}
