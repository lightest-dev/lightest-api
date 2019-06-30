using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace Lightest.Data.Models
{
    public class ServerChecker
    {
        [Required]
        public Guid CheckerId { get; set; }

        [Required]
        public string ServerIp { get; set; }

        [JsonIgnore]
        public TestingServer Server { get; set; }

        [JsonIgnore]
        public Checker Checker { get; set; }
    }
}
