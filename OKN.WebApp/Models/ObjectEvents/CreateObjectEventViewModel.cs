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
        
        [JsonProperty("files")]
        public List<FileInfoViewModel> Files { get; set; }
        
        [JsonProperty("photos")]
        public List<FileInfoViewModel> Photos { get; set; }
    }
}

