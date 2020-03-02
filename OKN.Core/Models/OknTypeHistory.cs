using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace OKN.Core.Models
{
    public class OknTypeHistory
    {
        [BsonElement("type")]
        public EObjectType Type { get; set; }

        public string Reason { get; set; }

        [JsonProperty("occuredAt")]
        public DateTime OccuredAt { get; set; }
    }
}
