using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace OKN.WebApp.Models.ObjectEvents
{
    public class CreateObjectEventViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("occuredAt")]
        public DateTime? OccuredAt { get; set; }

        [JsonProperty("links")]
        public List<EventLinkViewModel> Links { get; set; }

        [JsonProperty("images")]
        public List<EventImageViewModel> Images { get; set; }
    }

    public class EventLinkViewModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class EventImageViewModel : EventLinkViewModel { }
}

