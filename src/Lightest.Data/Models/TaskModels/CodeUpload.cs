using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Lightest.Data.Models.TaskModels
{
    public class CodeUpload : IUpload
    {
        [Key]
        public Guid UploadId { get; set; }

        [Required]
        public string Code { get; set; }

        [JsonIgnore]
        public virtual Language Language { get; set; }

        [Required]
        public Guid LanguageId { get; set; }

        [JsonIgnore]
        public string Message { get; set; }

        [JsonIgnore]
        public double Points { get; set; }

        [JsonIgnore]
        public string Status { get; set; }

        [JsonIgnore]
        public virtual TaskDefinition Task { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [JsonIgnore]
        public bool TestingFinished { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        [JsonIgnore]
        public string UserId { get; set; }
    }
}
