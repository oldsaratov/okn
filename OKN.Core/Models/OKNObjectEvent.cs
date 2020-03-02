using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class OknObjectEvent
    {
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public EObjectEventType Type { get; set; }

        [JsonProperty("occuredAt")]
        public DateTime OccuredAt { get; set; }

        [JsonProperty("createdBy")]
        public UserInfo Author { get; set; }
        
        [JsonProperty("photos")]
        public List<FileInfo> Photos { get; set; }
        
        [JsonProperty("files")]
        public List<FileInfo> Files { get; set; }
    }
}