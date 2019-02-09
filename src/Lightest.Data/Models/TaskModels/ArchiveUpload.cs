using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lightest.Data.Models.TaskModels
{
    public class ArchiveUpload : IUpload
    {
        [Key]
        public Guid UploadId { get; set; }

        [Required]
        public byte[] File { get; set; }

        [JsonIgnore]
        public virtual Language Language { get; set; }

        [Required]
        public Guid LanguageId { get; set; }

        public string Message { get; set; }

        [JsonIgnore]
        public double Points { get; set; }

        [JsonIgnore]
        public string Status { get; set; }

        [JsonIgnore]
        public virtual TaskDefinition Task { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        public bool TestingFinished { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public DateTime UploadDate { get; set; }
    }
}
