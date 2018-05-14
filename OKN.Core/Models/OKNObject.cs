using System;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class OKNObject
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
    }
    
    public class VersionInfo
    {
        [JsonProperty("version")]
        public long Version { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreateDate { get; set; }
        
        [JsonProperty("createdBy")]
        public string Author { get; set; }
    }
}