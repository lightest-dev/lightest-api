using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zetester.Data.Models
{
    public class CodeUpload
    {
        [BsonId]
        public string UploadId { get; set; }
        
        public string Code { get; set; }

        public string Language { get; set; }
    }
}
