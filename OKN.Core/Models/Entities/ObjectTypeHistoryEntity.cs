using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace OKN.Core.Models.Entities
{
    public class ObjectTypeHistoryEntity
    {
        [BsonElement("type")]
        public EObjectType Type { get; set; }

        [BsonElement("reason")]
        public string Reason { get; set; }

        [JsonProperty("occuredAt")]
        public BsonDateTime OccuredAt { get; set; }
    }
}
