using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models
{
    public class ContestSettings
    {
        [Key]
        public Guid CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category Category { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public TimeSpan? Length { get; set; }

        public bool Upsolving { get; set; }

        [JsonIgnore]
        public static ContestSettings Default => new ContestSettings
        {
            Length = TimeSpan.FromHours(2)
        };
    }
}
