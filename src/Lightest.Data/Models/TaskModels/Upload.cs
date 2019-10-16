using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models.TaskModels
{
    public class Upload
    {
        [Key]
        public Guid Id { get; set; }

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

        [JsonIgnore]
        public DateTime UploadDate { get; set; }
    }
}
