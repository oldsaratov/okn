using System.Collections.Generic;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class OknObject
    {
        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }
        
        [JsonProperty("type")]
        public EObjectType Type { get; set; }
        
        [JsonProperty("versionInfo")]
        public VersionInfo Version { get; set; }

        [JsonProperty("events")]
        public List<OknObjectEvent> Events { get; set; }
        
        [JsonProperty("mainPhoto")]
        public FileInfo MainPhoto { get; set; }
        
        [JsonProperty("photos")]
        public List<FileInfo> Photos { get; set; }
    }
}