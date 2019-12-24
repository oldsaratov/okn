using System;
using Newtonsoft.Json;

namespace OKN.WebApp.Models.ObjectEvents
{
    public class UpdateObjectEventViewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("occuredAt")]
        public DateTime? OccuredAt { get; set; }
    }
}