using MongoDB.Bson.Serialization.Attributes;

namespace Lightest.Data.Models
{
    public class CodeUpload
    {
        [BsonId]
        public string UploadId { get; set; }

        public string Code { get; set; }

        public string Language { get; set; }
    }
}