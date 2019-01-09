using System;
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
        public List<OKNObjectEvent> Events { get; set; }
    }
    
    public class VersionInfo
    {
        [JsonProperty("versionId")]
        public long VersionId { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreateDate { get; set; }
        
        [JsonProperty("createdBy")]
        public UserInfo Author { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}