using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
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
        public string Latitude { get; set; }
        
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        
        [JsonProperty("type")]
        public EObjectType Type { get; set; }

        [BsonElement("typeHistory")]
        public List<OknTypeHistory> TypeHistory { get; set; }

        [JsonProperty("versionInfo")]
        public VersionInfo Version { get; set; }

        [JsonProperty("events")]
        public List<OknObjectEvent> Events { get; set; }

        [JsonProperty("lastEvent")]
        public OknObjectEvent LastEvent { get; set; }

        [JsonProperty("eventsCount")]
        public int EventsCount { get; set; }

        [JsonProperty("mainPhoto")]
        public FileInfo MainPhoto { get; set; }
        
        [JsonProperty("photos")]
        public List<FileInfo> Photos { get; set; }
    }
}