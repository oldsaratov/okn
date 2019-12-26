using System;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class VersionInfo
    {
        [JsonProperty("versionId")]
        public long VersionId { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreateDate { get; set; }
        
        [JsonProperty("createdBy")]
        public UserInfo Author { get; set; }
    }
}