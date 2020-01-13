using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Lightest.Data.Mongo.Models
{
    public class UploadData
    {
        [BsonId]
        public Guid Id { get; set; }

        public string Code { get; set; }

        public byte[] Project { get; set; }
        
        public UploadData(){}

        public UploadData(Guid uploadId, string code)
        {
            Id = uploadId;
            Code = code;
        }

        public UploadData(Guid uploadId, byte[] project)
        {
            Id = uploadId;
            Project = project;
        }
    }
}
