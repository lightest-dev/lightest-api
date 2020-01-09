using System;
namespace Lightest.Data.Mongo.Models
{
    public class Mongo
    {
        public Guid Id { get; set; }
        
        public string Code { get; set; }
        
        public byte[] Hold { get; set; }
    }
}
