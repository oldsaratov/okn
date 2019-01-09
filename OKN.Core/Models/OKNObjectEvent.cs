using System;
using System.Collections.Generic;
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

        [JsonProperty("links")]
        public List<OKNObjectEventLink> Links { get; set; }

        [JsonProperty("images")]
        public List<OKNObjectEventImage> Images { get; set; }
    }

    public class OKNObjectEventLink
    {
        public OKNObjectEventLink(string description, string url)
        {
            Description = description;
            Url = url;
        }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class OKNObjectEventImage : OKNObjectEventLink {
        public OKNObjectEventImage(string description, string url) : base(description, url)
        {
        }
    }
}