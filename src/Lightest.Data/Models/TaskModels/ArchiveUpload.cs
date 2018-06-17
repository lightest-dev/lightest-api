using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Data.Models.TaskModels
{
    public class ArchiveUpload
    {
        [Key]
        public string UploadId { get; set; }

        public byte[] File { get; set; }

        public double Points { get; set; }

        public int LanguageId { get; set; }

        [JsonIgnore]
        public virtual Language Language { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        public int TaskId { get; set; }

        [JsonIgnore]
        public virtual Task Task { get; set; }
    }
}