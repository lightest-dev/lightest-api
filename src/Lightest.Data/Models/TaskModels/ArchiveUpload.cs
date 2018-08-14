namespace Lightest.Data.Models.TaskModels
{
    public class ArchiveUpload : IUpload
    {
        [Key]
        public int UploadId { get; set; }

        [Required]
        public byte[] File { get; set; }

        public string Message { get; set; }

        public bool TestingFinished { get; set; }

        [JsonIgnore]
        public double Points { get; set; }

        [JsonIgnore]
        public string Status { get; set; }

        [Required]
        public int LanguageId { get; set; }

        [JsonIgnore]
        public virtual Language Language { get; set; }

        public string UserId { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int TaskId { get; set; }

        [JsonIgnore]
        public virtual TaskDefinition Task { get; set; }
    }
}