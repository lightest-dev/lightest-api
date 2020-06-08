using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json.Serialization;

namespace Lightest.Data.Models
{
    public class TestingServerInfo
    {
        [Key]
        public string Ip { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        [ConcurrencyCheck]
        public ServerStatus Status { get; set; }

        [Required]
        public string Version { get; set; }

        [NotMapped]
        public IPAddress IPAddress => IPAddress.Parse(Ip);

        [JsonIgnore]
        public virtual ICollection<ServerChecker> Checkers { get; set; }
    }
}
