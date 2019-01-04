using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lightest.TestingService.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FileRequestType
    {
        [EnumMember(Value = "upload")]
        Upload = 1,
        [EnumMember(Value = "file")]
        File = 2,
        [EnumMember(Value = "tests")]
        TestCleanup = 4
    }
}
