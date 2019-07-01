using System;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class OKNObjectEvent
    {
        [JsonProperty("eventId")]
        public string EventId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("occuredAt")]
        public DateTime OccuredAt { get; set; }

        [JsonProperty("createdBy")]
        public UserInfo Author { get; set; }
    }
}